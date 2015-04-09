using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raw_programs
{
   public class PersonBase
    {
        protected System.Web.UI.WebControls.Label lblResults;
        protected System.Web.UI.WebControls.ListBox lstAuthor = new ListBox();

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDatabase();
        }
        private void LoadDatabase()
        {

            string selectSQL;
            selectSQL = "select Zbooktitle, Zbookauthor from zbkbookinfo";

            // Define the ADO.NET objects.
            SQLiteConnection con = new SQLiteConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SQLiteCommand cmd = new SQLiteCommand(selectSQL, con);
            SQLiteDataReader reader;

            // Try to open database and read information.
            try
            {
                con.Open();
                reader = cmd.ExecuteReader();

                // For each item, add the author name to the displayed
                // list box text, and store the unique ID in the Value property.
                //while (reader.Read())
                //{
                //    ListItem newItem = new ListItem();
                //    newItem.Text = reader[3] + ", " + reader[2];               
                //    lstAuthor.Items.Add(newItem);

                //}

                //foreach (ListItem p in lstAuthor.Items)
                //{
                //    Label1.Text += "<br/>" + p.Text;
                //}
                DataTable dt = new DataTable();
                dt.Load(reader);

                //  for r in dt.AsEnumerable() Select r; 
                //foreach (DataRow drow in dt.Rows)
                //{
                //    if (drow[0].ToString() == "Steve Jobs")
                //    {
                //        Label1.Text += "<br/>" + drow[4].ToString();
                //    }
                //    else { continue; }
                //}

                IEnumerable rquery = from row in dt.AsEnumerable()
                                     // group row by row.Field<string>("ZBOOKTITLE") into rowGroup
                                     select new
                                     {
                                         NAME = row.Field<string>("ZNAME"),
                                         Author = row.Field<string>("ZBOOKAUTHOR"),
                                         Chapter = row.Field<string>("ZNAME"),
                                         HIGHLIGHTCOLOR = (row.Field<Int64>("ZHIGHLIGHTCOLOR")).ToString(),
                                         TEXTUALCONTEXT = row.Field<string>("ZTEXTUALCONTEXT"),

                                     };
                //group table by new { column1 = table["Column1"], column2 = table["Column2"] }
                //    into groupedTable
                //    select new
                //    {
                //        x = groupedTable.Key,  // Each Key contains column1 and column2
                //        y = groupedTable.Count()
                //    };
                GridView1.DataSource = rquery;
                GridView1.DataBind();

                reader.Close();
            }
            catch (Exception err)
            {
                Label1.Text = "Error reading list of names. ";
                Label1.Text += err.Message;
            }
            finally
            {
                con.Close();
            }
        }
    }
}