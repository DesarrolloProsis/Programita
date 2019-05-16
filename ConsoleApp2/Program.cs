using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp2.Models;

namespace ConsoleApp2
{
    class Program
    {

        static void Main(string[] args)
        {

            string SQL = "Data Source =.; Initial Catalog = GTDB; Integrated Security = True";

            SqlConnection ConexionSQL = new SqlConnection(SQL);

            using (SqlCommand SqlCommand = new SqlCommand("Select Tag From Historico group by Tag", ConexionSQL))
            {

                try
                {

                    ConexionSQL.Open();
                    SqlCommand.ExecuteNonQuery();
                    SqlDataAdapter sqlData = new SqlDataAdapter(SqlCommand);
                    DataTable table = new DataTable();
                    sqlData.Fill(table);
                    List<Primero> Lista1 = new List<Primero>();

                    foreach(DataRow item in table.Rows)
                    {
                        Lista1.Add(new Primero
                        {                            
                            Tag = Convert.ToString(item["Tag"])

                        });
                    }   
                    

                    foreach(var item2 in Lista1)
                    {
                        SqlCommand.CommandText = "Select CuentaId, NumTag, NumCuenta, StatusTag, StatusCuenta, TypeCuenta, SaldoCuenta, SaldoTag " +
                                                   "From Tags t Inner Join CuentasTelepeajes c on t.CuentaId = c.Id Where t.NumTag = '" + item2.Tag + "'";

                        SqlCommand.ExecuteNonQuery();
                        SqlDataAdapter sqlData2 = new SqlDataAdapter(SqlCommand);
                        DataTable table2 = new DataTable();
                        sqlData2.Fill(table2);

                        if (table2.Rows.Count != 0)
                        {

                            if (table2.Rows[0]["TypeCuenta"].ToString() == "Individual")
                            {

                                SqlCommand.CommandText = "Select Sum(CAST(Monto as float)) from OperacionesCajeroes where Numero = '" + item2.Tag + "'";
                                double SaldoTotal = Convert.ToDouble(SqlCommand.ExecuteScalar());

                                SqlCommand.CommandText = "Select Id, Saldo, Tag, SaldoAnterior, SaldoActualizado from Historico where Tag = '" + item2.Tag +"'";

                                SqlCommand.ExecuteNonQuery();
                                SqlDataAdapter sqlData3 = new SqlDataAdapter(SqlCommand);
                                DataTable table3 = new DataTable();
                                sqlData3.Fill(table3);

                                List<Segundo> Lista2 = new List<Segundo>();

                                foreach(DataRow item3 in table3.Rows)
                                {
                                    Lista2.Add(new Segundo
                                    {
                                        Id = Convert.ToInt64(item3["Id"]),
                                        NumTag = Convert.ToString(item3["Tag"]),
                                        Saldo = Convert.ToDouble(item3["Saldo"]),
                                        saldoNuevo = Convert.ToDouble(item3["SaldoAnterior"]),
                                        saldoViejo = Convert.ToDouble(item3["SaldoActualizado"])

                                    });
                                }
                                
                                foreach(var item4 in Lista2)
                                {
                                    if (item4.saldoViejo == 0 && item4.saldoNuevo == 0)
                                    {
                                        SqlCommand.CommandText = "update Historico set SaldoAnterior = '" + Convert.ToString(SaldoTotal) + "' where Id = '" + item4.Id + "'";
                                        SqlCommand.ExecuteNonQuery();
                                        SaldoTotal = SaldoTotal - item4.Saldo;
                                        SqlCommand.CommandText = "update Historico set SaldoActualizado = '" + Convert.ToString(SaldoTotal) + "' where Id = '" + item4.Id + "'";
                                        SqlCommand.ExecuteNonQuery();
                                    }                                    


                                }

                                

                            }
                            else
                            {
                                var Cuenta = table2.Rows[0]["NumCuenta"].ToString();
                                var CuentaID = table2.Rows[0]["CuentaId"].ToString();

                                SqlCommand.CommandText = "Select Sum(CAST(Monto as float)) from OperacionesCajeroes where Numero = '"+Cuenta+"'";
                                double SaldoTotal = Convert.ToDouble(SqlCommand.ExecuteScalar());

                                SqlCommand.CommandText = "Select h.Id, h.Tag, h.Saldo, h.SaldoAnterior, h.SaldoActualizado from Tags t inner join Historico h on t.NumTag = h.Tag where t.CuentaId = '" + CuentaID + "'";

                                SqlCommand.ExecuteNonQuery();
                                SqlDataAdapter sqlData4 = new SqlDataAdapter(SqlCommand);
                                DataTable table4 = new DataTable();
                                sqlData4.Fill(table4);

                                List<Segundo> Lista3 = new List<Segundo>();

                                foreach (DataRow item5 in table4.Rows)
                                {
                                    Lista3.Add(new Segundo
                                    {
                                        Id = Convert.ToInt64(item5["Id"]),
                                        NumTag = Convert.ToString(item5["Tag"]),
                                        Saldo = Convert.ToDouble(item5["Saldo"]),
                                        saldoNuevo = Convert.ToDouble(item5["SaldoAnterior"]),
                                        saldoViejo = Convert.ToDouble(item5["SaldoActualizado"])

                                    });
                                }

                                foreach (var item6 in Lista3)
                                {
                                    if (item6.saldoViejo == 0 && item6.saldoNuevo == 0)
                                    {
                                        SqlCommand.CommandText = "update Historico set SaldoAnterior = '" + Convert.ToString(SaldoTotal) + "' where Id = '" + item6.Id + "'";
                                        SqlCommand.ExecuteNonQuery();
                                        SaldoTotal = SaldoTotal - item6.Saldo;
                                        SqlCommand.CommandText = "update Historico set SaldoActualizado = '" + Convert.ToString(SaldoTotal) + "' where Id = '" + item6.Id + "'";
                                        SqlCommand.ExecuteNonQuery();
                                    }


                                }

                             

                            }

                        }

                    }
                    

                }
                catch(Exception ex)
                {
                    string path = @"C:\DescuentosListas\";
                    string archivo = "Programita.txt";
                    using (StreamWriter file = new StreamWriter(path + archivo, true))
                    {
                        file.WriteLine(ex.Message);
                        file.WriteLine("\nStackTrace ---\n{0}", ex.StackTrace);
                        file.WriteLine(" //se agrega información al documento");
                        file.Dispose();
                        file.Close();
                    }
                }

                ConexionSQL.Close();

            }

        }
    }
}
