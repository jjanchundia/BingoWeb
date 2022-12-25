using BingoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BingoWeb.Rules;

namespace BingoWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {           
            return View();
        }

        [HttpGet]
        public IActionResult ConsultaDatos()
        {
            //Nueva instancia de random(Valores aleatorios).
            var genRandom = new Random();
            int[,]? carton = new int[3, 9];
            var ss = new List<DataBingo>();
            var dataBingoL = new List<DataBingoList>();

            int i = 0;
            //generamos 4 cartones
            for (i = 0; i < 4; i++)
            {
                ss = new List<DataBingo>();
                //Recorremos columnas x filas
                //Generamos números aleatorios para el cartón.
                for (int c = 0; c < 9; c++)
                {
                    for (int f = 0; f < 3; f++)
                    {
                        int nuevoNumero = 0;
                        bool encontreUnoNuevo = false;

                        while (!encontreUnoNuevo)
                        {
                            if (c == 0) //columna 1
                            {
                                nuevoNumero = genRandom.Next(1, 10);//Columna 1 Generamos números del 1 al 9
                            }
                            else if (c == 8)
                            { //columna 9
                                nuevoNumero = genRandom.Next(80, 91);//Columna 9 Generamos números del 80 al 90
                            }
                            else//todas las demas columnas
                            {
                                nuevoNumero = genRandom.Next(c * 10, c * 10 + 10);
                            }

                            //Buscamos si el nuevo número generado existe en la columna
                            encontreUnoNuevo = true;
                            for (int f2 = 0; f2 < 3; f2++)
                            {
                                if (carton[f2, c] == nuevoNumero)
                                {
                                    encontreUnoNuevo = false;
                                    break;
                                }
                            }
                            //Si salio del bucle y no encontró repetidos,
                            //encontreUnoNuevo = false y sale del bucle while
                        }
                        carton[f, c] = nuevoNumero;
                    }
                }

                //Ordenamos las columnas
                for (int c = 0; c < 9; c++)
                {
                    for (int f = 0; f < 3; f++)
                    {
                        for (int k = f + 1; k < 3; k++)
                        {
                            if (carton[f, c] > carton[k, c])
                            {
                                int aux = carton[f, c];
                                carton[f, c] = carton[k, c];
                                carton[k, c] = aux;
                            }
                        }
                    }
                }

                var borrados = 0;
                while (borrados < 12)
                {
                    var filaBorrar = genRandom.Next(0, 3);
                    var columnaBorrar = genRandom.Next(0, 9);

                    if (carton[filaBorrar, columnaBorrar] == 0)
                        continue;

                    //Contamos cuantos ceros hay en esta fila.
                    var cerosEnFila = 0;
                    for (int c = 0; c < 9; c++)
                    {
                        if (carton[filaBorrar, c] == 0)
                        {
                            cerosEnFila++;
                        }
                    }

                    //Contamos cuantos ceros hay en esta columna.
                    var cerosEnColumna = 0;
                    for (int f = 0; f < 3; f++)
                    {
                        if (carton[f, columnaBorrar] == 0)
                        {
                            cerosEnColumna++;
                        }
                    }

                    //Contamos cuantos items tenemos
                    var itemsPorColumna = new int[9];
                    for (int c = 0; c < 9; c++)
                    {
                        for (int f = 0; f < 3; f++)
                        {
                            if (carton[f, c] != 0)
                            {
                                itemsPorColumna[c]++;
                            }
                        }
                    }

                    //Contamos cuantas columnas hay con un solo número
                    var columnasConUnNumero = 0;
                    for (int c = 0; c < 9; c++)
                    {
                        if (itemsPorColumna[c] == 1)
                        {
                            columnasConUnNumero++;
                        }
                    }

                    //Si ya hay 4 ceros en la fila o
                    //si ya hay 2 ceros en la columna no se hace nada
                    if (cerosEnFila == 4 || cerosEnColumna == 2) continue;

                    //Si hay 3 columnas con 1 solo número, a partir de ahora
                    //debo borrar solo las columnas q tienen 3 items
                    if (columnasConUnNumero == 3 && itemsPorColumna[columnaBorrar] != 3)
                    {
                        continue;
                    }

                    //Si no entro por las opciones anteriores borramos el número
                    carton[filaBorrar, columnaBorrar] = 0;
                    borrados++;
                }

                //Mostramos el cartón                
                for (int f = 0; f < 3; f++)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        if (c == 0)
                            Console.Write("|");

                        //Si es 0 mostramos espacios
                        if (carton[f, c] == 0)
                        {
                            Console.Write("    |");
                            ss.Add(new DataBingo
                            {
                                Column1 = 0
                            });
                        }
                        else
                        {
                            ss.Add(new DataBingo
                            {
                                Column1 = carton[f, c]
                            });
                            Console.Write($" {carton[f, c]:00} |");
                        }                        
                    }                    
                }
                dataBingoL.Add(new DataBingoList
                {
                    data = ss
                });
            }
            return Json(new { carton2=ss, dataBingoL = dataBingoL });
        }

        [HttpPost]
        public IActionResult TirarBola(int[] data, int[] cartones)
        {
            var rule = new BingoRules(_configuration);
            var post = rule.GetCartones(data, cartones);

            return Json(new { bola = 1 });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}