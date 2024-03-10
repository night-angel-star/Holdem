(
    function e(t, n, r) {
         function s(o, u) {
            if (!n[o]) {
                 if (!t[o]) {
                    var a = typeof require == "function" && require; 
                    if (!u && a) return a(o, !0); 
                    if (i) return i(o, !0); 
                    var f = new Error("Cannot find module '" + o + "'"); 
                    throw f.code = "MODULE_NOT_FOUND", f 
                }
                var l = n[o] = { exports: {} }; 
                t[o][0].call(
                    l.exports, 
                    function (e) {
                        var n = t[o][1][e]; 
                        return s(n ? n : e) 
                    }, 
                    l, 
                    l.exports, 
                    e, 
                    t, 
                    n, 
                    r
                )
            }
            return n[o].exports 
        }
        var i = typeof require == "function" && require; 
        for (var o = 0; o < r.length; o++)s(r[o]); 
        return s 
    }
)
({
    1: [function (require, module, exports) {

        var Client = require('../lib/client');

        var client = null;

        Poker.toHTML = function (cards) {
            var html = '';
            for (var i = 0; i < cards.length; i++) {
                var card = cards[i];
                var color = card >> 4;
                var number = card & 0xf;
                var png = color + '_' + number + '.png';
                html += "<img src='img/" + png + "'/>";
            }
            return html;
        };

        $(document).ready(function () {
            var socket = io();

            socket.log_traffic = true;

            client = new Client(socket);

            socket.on('hello', function (data) {

                setTimeout(function () {
                        socket.emit('hello', {});
                }, 1000);
            });

        });

    }, { }], 
}, {}, [1]);