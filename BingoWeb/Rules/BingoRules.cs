using Dapper;
using BingoWeb.Models;
using System.Data.SqlClient;

namespace BingoWeb.Rules
{
    public class BingoRules
    {
        private readonly IConfiguration _configuration;
        public BingoRules(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<HistorialCartones> GetCartones(int[] dataBingo, int[] cartonLleno)
        {
            string connectionString = _configuration.GetConnectionString("BingoWeb");

            using (var connection2 = new SqlConnection(connectionString))
            {
                connection2.Open();
                
                //Llenamos modelo de datos, el cual pasaremos a columnas para llenar tabla HistorialCartones.
                var data = new HistorialCartones
                {
                    FechaHora = DateTime.Now,
                    Carton1 = cartonLleno[0],
                    Carton2 = cartonLleno.Length == 2 ? cartonLleno[1] : null,
                    Carton3 = cartonLleno.Length == 3 ? cartonLleno[2] : null,
                    Carton4 = cartonLleno.Length == 4 ? cartonLleno[3] : null,
                };

                //Procedemos a llenar tabla HistorialCartones(Cabecera).
                var queryInsert = "INSERT INTO HistorialCartones(FechaHora, Carton1, Carton2, Carton3, Carton4) " +
                    "Values(@fechaHora, @carton1, @carton2, @carton3, @carton4)";
                var result = connection2.Execute(queryInsert, new
                {
                    data.FechaHora,
                    data.Carton1,
                    data.Carton2,
                    data.Carton3,
                    data.Carton4,
                });

                //Obtenemos último IdHistorialCarton insertado para pasarle a insert de tabla HistorialBolillero
                var query = @$"Select top 1 IdHistorialCarton from HistorialCartones order by idHistorialCarton desc";
                var post = connection2.Query<HistorialCartones>(query);

                //Llenado de tabla HistorialBolillero
                foreach (var item in dataBingo)
                {
                    var dataHistorialBolillero = new HistorialBolillero
                    {
                        IdHistorialCarton = post.FirstOrDefault().IdHistorialCarton,
                        FechaHora = DateTime.Now,
                        numeroBolilla = item,
                    };
                    var queryInsertHB = "INSERT INTO HistorialBolillero(IdHistorialCarton, FechaHora, numeroBolilla) " +
                        "Values(@idHistorialCarton, @fechaHora, @numeroBolilla)";
                    var resultHB = connection2.Execute(queryInsertHB, new
                    {
                        dataHistorialBolillero.IdHistorialCarton,
                        dataHistorialBolillero.FechaHora,
                        dataHistorialBolillero.numeroBolilla
                    });
                }
            }

            return new List<HistorialCartones>();
        }
    }
}