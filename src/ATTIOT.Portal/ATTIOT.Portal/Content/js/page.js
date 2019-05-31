$(function () {
    var dotnet = $(".dot_net ul .pro_name").length;
    var java = $(".java ul .pro_name").length;
    $("#dotnet_total").html(dotnet);
    $("#java_total").html(java);
    banner_slide();
    $('#user_list').modal('hide');
    var limit = $(".server").offset().top + 1;
    $(window).scroll(function () {
        var top = $(window).scrollTop();
        if (top >= limit) {
            $("#backtotop").fadeIn();
        }
        else {
            $("#backtotop").fadeOut();
        }
    });
    $.ajax({
        url: "/home/getprojectlist",
        type: "post",
        dataType: "json",
        async: false,
        success: function (json) {
            if (json != null && json != "" && json.length > 0) {
                $(".s_box").autocomplete({
                    source: json,  //此处由于数据较少，一次取出，数据较多时需做异步数据过滤
                    focus: function (event, ui) {
                        $("#pro_id").val(ui.item.id);
                    },
                    select: function (event, ui) {
                        $(".search_info").hide();
                        go_position(ui.item.id);
                    }
                });
            }
        }
    });

    $(".s_box").keypress(function () {
        $(".search_info").hide();
        var value = $("#pro_id").val();
        if (value != "") {
            $("#pro_id").val("");
        }
    });
});

function go_position(id) {
    if (id == "") {
        return;
    }
    var exp = ".list ul > li[no='" + id + "']";
    var item = $(exp);
    if (item.length > 0) {
        $('html, body').animate({
            scrollTop: item.offset().top
        }, 300);
    }
}

$(".s_btn").click(function () {
    var name = $(".s_box").val();
    var id = $("#pro_id").val();
    if (name == "") {
        $(".search_info").html("请输入项目名称").show();
        return;
    }
    if (id == "") {
        $(".search_info").html("无此项目").show();
        return;
    }
    $(".search_info").hide();
    go_position(id);
});

//锚点平滑滚动
$(".nav li a").click(function () {
    var attr = $(this).attr("anchor");
    var exp = ".data > ." + attr;
    var target = $(exp);
    $('html, body').animate({
        scrollTop: target.offset().top + 1
    }, 500);
});
//下载项目清单
$("#down_btn").click(function () {
    var url = "/home/download";
    location.href = url;
});
//幻灯片
function banner_slide() {
    var $img = $("#img-fade .image li");
    var $sign = $("#img-fade .sign li");
    var len = $img.length;
    var out_speed = 1500;
    var in_speed = 900;
    var text_speed = 800;
    var interval = 10000;
    $img.first().show().find(".text").slideDown(text_speed);
    var i = 0;
    function turn() {
        if (i < len - 1) {
            $img.eq(i).fadeOut(out_speed).next().fadeIn(in_speed);
            i++;
        }
        else {
            $img.eq(i).fadeOut(out_speed);
            $img.eq(0).fadeIn(in_speed);
            i = 0;
        }
        $img.eq(i).find(".text").slideDown(text_speed);
        $img.eq(i).siblings("li").find(".text").hide();
        $sign.eq(i).addClass("on").siblings("li").removeClass("on");
    }
    $("#img-fade .sign li").click(function () {
        clearInterval(set);
        var index = $(this).index();
        if (index != i) {
            $img.eq(i).fadeOut(out_speed);
            $img.eq(index).fadeIn(in_speed).find(".text").slideDown(text_speed);
            $img.eq(index).siblings("li").find(".text").hide();
            $sign.eq(index).addClass("on").siblings("li").removeClass("on");
            i = index;
            set = setInterval(turn, interval);
        }
    });
    set = setInterval(turn, interval);
}
//回到顶部
$("#backtotop").click(function () {
    $('html,body').animate({
        scrollTop: 0
    }, 300);
});
//更多
$("#more_account").click(function () {
    $.ajax({
        url: "/home/getuserlist",
        type: "post",
        dataType: "json",
        async: false,
        success: function (json) {
            var content = "";
            var pw_cookie = $.cookie('pw_cookie');
            if (pw_cookie == undefined || pw_cookie == "") {
                $.cookie('pw_cookie', false, { expires: 7 })  //设置初始cookie
            }
            if (json != null && json != "" && json.length > 0) {
                content += "<div class='pw_view'><span class='title'>显示密码：</span><input id='create-switch' type='checkbox' ></div><div class='clearfix'></div>";
                content += "<table class='table table-bordered table-striped'><thead><tr><th>服务器</th><th>登录帐号</th><th>密码</th></tr></thead><tbody>";
                for (var i = 0; i < json.length; i++) {
                    content += "<tr>";
                    content += "<td>" + json[i].IP + "</td>";
                    content += "<td>" + json[i].User + "</td>";
                    content += "<td style='width:200px;'><span class='encrypt'>****</span>" + "<span class='pw_val'>" + json[i].Password + "</span></td>";
                    content += "</tr>";
                }
                content += "</tbody></table>";
                var tip = "此处公开服务器登录帐号是方便相关人员更好地完成工作，请妥善保管好帐号";
                content += "<div class='u_tip'>" + tip + "<div>";
                $('#create-switch').bootstrapSwitch('destroy'); //先销毁，再创建
                $("#user_data").html(content);
                $('#create-switch').wrap('<div id="switch-view" class="switch switch-small" data-on-label="是" data-off-label="否" />').parent().bootstrapSwitch();
                var def_cookie = $.cookie('pw_cookie');
                if (def_cookie != undefined && def_cookie == "true") {
                    $("#user_list .encrypt").hide();
                    $("#user_list .pw_val").show();
                    $('#switch-view').bootstrapSwitch('setState', true);
                }
                $('#user_list').modal('show');
                //是否显示密码
                $("#switch-view").on('switch-change', function (e, data) {
                    if (data.value) {
                        $("#user_list .encrypt").hide();
                        $("#user_list .pw_val").show();
                    }
                    else {
                        $("#user_list .encrypt").show();
                        $("#user_list .pw_val").hide();
                    }
                    $.cookie('pw_cookie', data.value, { expires: 7 });
                });
            }
        }
    });
});