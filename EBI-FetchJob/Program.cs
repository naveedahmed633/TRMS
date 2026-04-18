using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBI_FetchJob
{

    class Program
    {
        public static void Main(string[] args)
        {
            List<int> insertedIDs = new List<int>();

            HaTransitEBI[] allActive = null;
            List<HaTransitEBI> ebiTransit = new List<HaTransitEBI>();

            try
            {
                //string ConnectionString = "SERVER=103.4.92.87;DATABASE=shadow;User Id=root;PASSWORD=root;SslMode =none;port=3306;Connect timeout=9600;";
                string strConnectionString = Context.GetConnectionstring();

                using (MySqlConnection myConnection = new MySqlConnection(strConnectionString))
                {
                    Console.WriteLine("Opening Remote Server Connection...");
                    myConnection.Open();
                    Console.WriteLine("Connection Opened...");

                    Console.WriteLine("Step 1: Read Remote Server Data...");
                    string query = "SELECT  * FROM unistthatransits WHERE active=1 AND C_Date >= '2018-11-01'  ORDER BY C_Date DESC LIMIT 0, 100000; ";
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, myConnection);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ebiTransit.Add(new HaTransitEBI
                        {
                            HaTransitId = Convert.ToInt32(reader["UTHaTransitId"]),
                            C_Name = reader["C_Name"].ToString(),
                            C_Unique = reader["C_Unique"].ToString(),
                            C_Date = Convert.ToDateTime(reader["C_Date"].ToString()),
                            C_Time = reader["C_Time"].ToString(),
                            L_UID = Convert.ToInt32(reader["L_UID"].ToString()),
                            L_TID = Convert.ToInt32(reader["L_TID"].ToString()),
                            active = Convert.ToBoolean(reader["active"].ToString())
                        });
                    }
                }
                //myConnection.Close();

                allActive = ebiTransit.ToArray();

                Console.WriteLine("Step 2: Insert Data into TimeTune DB...");
                if (allActive != null)
                {
                    using (var db = new DLL.Models.Context())
                    {
                        foreach (var entity in allActive)
                        {
                            DateTime? date = null;

                            if (entity.C_Date.HasValue)
                            {
                                insertedIDs.Add(entity.HaTransitId);
                                date = entity.C_Date.Value.Date;

                                if (entity.C_Unique != null && !entity.C_Unique.Equals(""))
                                {
                                   
                                    int temp;
                                    if (!int.TryParse(entity.C_Unique, out temp))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                if (entity.C_Unique.Length == 1)
                                {
                                    entity.C_Unique = "00000" + entity.C_Unique;
                                }
                                else if (entity.C_Unique.Length ==2)
                                {
                                    entity.C_Unique = "0000" + entity.C_Unique; ;
                                }
                                else if (entity.C_Unique.Length == 3)
                                {
                                    entity.C_Unique = "000" + entity.C_Unique; ;
                                }

                                else if (entity.C_Unique.Length == 4)
                                {
                                    entity.C_Unique = "00" + entity.C_Unique; ;
                                }

                                else if (entity.C_Unique.Length == 5)
                                {
                                    entity.C_Unique = "0" + entity.C_Unique; ;
                                }
                               
                               
                                db.ha_transit.Add(new DLL.Models.HaTransit()
                                {
                                    C_Date = date,
                                    C_Time = entity.C_Time,
                                    C_Name = entity.C_Name,
                                    C_Unique = entity.C_Unique,
                                    L_UID = entity.L_UID,
                                    L_TID = entity.L_TID,
                                    active = true
                                });
                            }
                        }

                        db.SaveChanges();
                    }


                    int _false = 0;

                    Console.WriteLine("Step 3: Update Flag of Remote Server Data...");
                    using (MySqlConnection myConnection = new MySqlConnection(strConnectionString))
                    {
                        myConnection.Open(); string updateQuery="";MySqlCommand cmd2 ;
                        for (int i = 0; i < allActive.Length; i++)
                        {
                            updateQuery = "UPDATE unistthatransits SET active=@num WHERE active=1 and UTHaTransitId=" + allActive[i].HaTransitId + "";
                            cmd2 = new MySqlCommand(updateQuery, myConnection);
                            cmd2.Parameters.AddWithValue("@num", _false);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    //myConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nEXCEPTION: StackTrace" + ex.StackTrace + ", Message: " + ex.Message);
            }

            Console.WriteLine("\n\nProcess COMPLETED: Data has been successfully transferred!");

            ///Console.ReadKey();
        }
    }
}
