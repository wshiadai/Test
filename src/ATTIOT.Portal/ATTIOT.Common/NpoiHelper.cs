using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.Util;
using System.Web;

namespace ATTIOT.Common
{
    public class NpoiHelper
    {

        private IWorkbook _workBook = null;  //excl对象

        private Dictionary<string, ISheet> _worksheets = null;

        private ISheet _currentSheet = null;

        public NpoiHelper()
        {

        }

        public NpoiHelper(string fullName)
        {
            using (FileStream file = new FileStream(fullName, FileMode.Open, FileAccess.Read))
            {

                if (fullName.IndexOf(".xlsx") > 0) // 2007版本
                    _workBook = new XSSFWorkbook(file);
                else// 2003版本
                    _workBook = new HSSFWorkbook(file);
            }

            //获取所有的工作簿
            for (int i = 0; i < _workBook.NumberOfSheets; i++)
            {
                _worksheets.Add(_workBook.GetSheetAt(i).SheetName, _workBook.GetSheetAt(i));
            }

            _currentSheet = _workBook.GetSheetAt(0); //默认第一个工作簿 
        }


        /// <summary>
        /// 判断指定行是否有数据
        /// </summary>
        /// <param name="row">
        /// 从1开始，为Excel行序号
        /// 指定行是否有数据（以连续50列没有数据为标准）
        /// </param>
        /// <returns></returns>
        public bool RowHasValue(int row)
        {
            bool r = false;
            for (int i = 0; i < 50; ++i)
            {
                string CellValue = _currentSheet.GetRow(row - 1).GetCell(i).StringCellValue;
                if (StringHelper.IsEmpty(CellValue))
                {
                    r = true;
                    break;
                }
            }
            return r;
        }





        public static DataTable ExclImprotDataTable(string fileMapth,int index)
        {
            DataTable dt = new DataTable();
            IWorkbook hssfworkbook;
            using (FileStream file = new FileStream(fileMapth, FileMode.Open, FileAccess.Read))
            {

                if (fileMapth.IndexOf(".xlsx") > 0) // 2007版本
                    hssfworkbook = new XSSFWorkbook(file);
                else// 2003版本
                    hssfworkbook = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfworkbook.GetSheetAt(index);
            if (sheet == null) { return null; }
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null || row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).ToString()))
                {
                    continue;
                }
                DataRow dataRow = dt.NewRow();
                int count = 0;
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                   
                    if (row.GetCell(j) != null && !string.IsNullOrEmpty(row.GetCell(j).ToString()))
                    {
                        if (row.GetCell(j).CellType == CellType.Numeric && HSSFDateUtil.IsCellDateFormatted(row.GetCell(j)))
                            dataRow[j] = row.GetCell(j).DateCellValue.ToString();
                        else
                            dataRow[j] = row.GetCell(j).ToString();
                    }
                    else
                    {
                        count++;
                    }
                }
                if (count < cellCount)
                {
                    dt.Rows.Add(dataRow);
                }
            }
            return dt;
        }
        #region 导出
        public static MemoryStream DataTableToExcel(DataTable dtSource, string strHeaderText, bool isHeader = true)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet(strHeaderText);
            HSSFCellStyle dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int startRow = 0;
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = (HSSFSheet)workbook.CreateSheet();
                    }

                    #region 列头及样式
                    {
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;

                        if (isHeader)
                        {
                            HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
                            CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1);  //合并单元格
                            sheet.AddMergedRegion(cellRangeAddress);
                            HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                            headStyle.VerticalAlignment = VerticalAlignment.Center;
                            headStyle.Alignment = HorizontalAlignment.Center;
                            HSSFFont headfont = (HSSFFont)workbook.CreateFont();
                            headfont.FontHeightInPoints = 10;
                            headfont.Boldweight = 700;
                            headStyle.SetFont(headfont);
                            headerRow.CreateCell(0).SetCellValue(strHeaderText);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            headStyle.SetFont(font);
                            startRow = 1;
                        }
                        HSSFRow columnRow = (HSSFRow)sheet.CreateRow(startRow);
                        HSSFCellStyle columnStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        HSSFFont firstFont = (HSSFFont)workbook.CreateFont();
                        firstFont.Boldweight = 700;
                        columnStyle.SetFont(firstFont);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            columnRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            columnRow.GetCell(column.Ordinal).CellStyle = columnStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 3) * 256);
                        }
                        // headerRow.Dispose();
                    }
                    if (isHeader)
                    {
                        rowIndex = 2;
                    }
                    else
                    {
                        rowIndex = 1;
                    }
                    #endregion
                }
                #endregion


                #region 填充内容
                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            System.DateTime dateV;
                            System.DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(drValue);

                            //  newCell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
        public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName, bool isHeader = true)
        {
            HttpContext curContext = HttpContext.Current;

            byte[] bytes = DataTableToExcel(dtSource, strHeaderText, isHeader).GetBuffer();
            // 设置编码和附件格式
            curContext.Response.ClearHeaders(); //清楚头信息
            curContext.Response.ContentType = "application/octet-stream";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                  "attachment;filename=" + curContext.Server.UrlEncode(strFileName));
            curContext.Response.AppendHeader("Content-Transfer-Encoding", "binary");
            curContext.Response.AppendHeader("Pragma", "public");
            curContext.Response.AppendHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
            curContext.Response.BinaryWrite(bytes);
            curContext.Response.Flush();
            curContext.Response.End();
        }

        #endregion


    }
}
