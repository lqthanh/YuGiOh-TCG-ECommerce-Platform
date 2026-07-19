using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE._iservices;
using BE.Controllers;
using BE.Model.Entity;
using BE.Model.ValueObject;
using Microsoft.AspNetCore.Mvc;

namespace BE._services
{
    public class ContentService : IContentService
    {
        public async Task<string> ContentWrite(string message)
        {
            return 
                "<head>" + 
                    "<script src='/assets/live2d/live2d.min.js'></script>" + 
                    "<script src='/assets/live2d/live2djs.js'></script>" + 
                    "<link rel='stylesheet' href='/assets/live2d/live2d.css'>" + 
                    "<style>" + 
                        "@import url('https://fonts.googleapis.com/css2?family=Inter&family=Poppins:wght@400;500;600;700&display=swap');" + 
                        "* {" + 
                        "font-family: 'Poppins';" + 
                        "box-sizing: border-box;" + 
                        "}" + 
                    "</style>" + 
                "</head>" + 
                "<body>" + 
                    "<h1 style='color:#7400CC;'> " + message + "</h1>" + 
                    "<a href='" + ApiEnvironment.ClientURL + "'>Go To Home Page</a>" + 
                "</body>";
        }

        public async Task<string> PriceChartWrite(List<Deal> deals)
        {
            string dealsArray = "";
            foreach(var item in deals)
            {
                dealsArray = dealsArray + "{ price: " + item.Price + ", date: '" + item.AcceptedDate + "' },";
            }
            return
                "" + 
                "<!DOCTYPE html>" + 
                "<html lang='en'>" + 
                "<head>" + 
                    "<meta charset='UTF-8'>" + 
                    "<meta name='viewport' content='width=device-width, initial-scale=1.0'>" + 
                    "<title>Card Price</title>" + 
                    "<script src='https://cdn.jsdelivr.net/npm/chart.js'></script>" + 
                "</head>" + 
                "<body>" + 
                    "<canvas id='myChart' width='400' height='100'></canvas>" + 
                    "<script>" + 
                        "var cards = [" + dealsArray + "];" + 
                        "var prices = cards.map(card => card.price);" + 
                        "var dates = cards.map(card => card.date);" + 
                        "var ctx = document.getElementById('myChart').getContext('2d');" + 
                        "var myChart = new Chart(ctx, {" + 
                            "type: 'line'," + 
                            "data: {" + 
                                "labels: dates," + 
                                "datasets: [" + 
                                    "{" + 
                                        "label: 'Price in this year'," + 
                                        "data: prices," + 
                                        "borderWidth: 1," + 
                                        "pointRadius: 3," + 
                                        "borderColor: 'rgba(116, 0, 204, 1)'," + 
                                        "pointBackgroundColor: 'rgba(116, 0, 204, 1)'," + 
                                        "fill: false" + 
                                    "}" + 
                                "]" + 
                            "}," + 
                            "options: {" + 
                                /*"legend: {" + 
                                    "display: false" + 
                                "}," + */ 
                                "scales: {" + 
                                    "x: {" + 
                                        "display: false" + 
                                    "}," + 
                                    "y: {" + 
                                        "beginAtZero: true" + 
                                    "}" + 
                                "}" + 
                            "}" + 
                        "});" + 
                    "</script>" + 
                "</body>" + 
                "</html>";
        }
    }
}