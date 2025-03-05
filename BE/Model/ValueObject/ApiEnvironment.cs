using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.Model.ValueObject
{
    public class ApiEnvironment
    {
        public const string ClientURL = "http://localhost:5173";
        public const int normalPrice = 1000;
        public const int deluxePrice = 5000;
        public const int waifuPrice = 2000;
        public const int discountPercent = 10;
        public const int defRegMoney = 10000;
        public const string live2dCanvas = "<canvas id='live2d' width='560' height='600' class='live2d' style='position: absolute; bottom: 0px; left: 0%; width: 32%;'></canvas>";
        public const string imageDeptrai = "/assets/image/deptrai.png";
        public const string imageNerd = "/assets/image/nerd.png";
        public const string imageBubble = "/assets/image/bubble.png";
    }
}