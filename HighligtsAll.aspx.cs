using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SQLite;
using System.Data;
using System.Collections;
using System.Configuration;
using Ionic.Zip;
using System.IO;

namespace Raw_programs
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        String con1user, con2user, BookData="";
        
        protected System.Web.UI.WebControls.Label lblResults;
        protected System.Web.UI.WebControls.ListBox lstAuthor = new ListBox();
        public  String sv="";
        protected void Page_Load(object sender, EventArgs e)
        {
          sv = Session["uid"].ToString();
          Read_DB();
          

        }
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                con1user = String.Format(ConfigurationManager.ConnectionStrings["bookinfo_conn"].ToString(), Session["uid"].ToString());
                con2user = String.Format(ConfigurationManager.ConnectionStrings["highlights_conn"].ToString(), Session["uid"].ToString());
            }
        }
        public void Read_DB()
        {
            //System.Data.SQLite.
            string select_BSQL, select_HSQL;
            select_BSQL = "select ZBOOKTITLE, ZBOOKAUTHOR,ZDATABASEKEY from ZBKBOOKINFO";
            //  select_HSQL = "select datetime((ZANNOTATIONMODIFICATIONDATE) + 978307200, 'unixepoch', 'localtime') as lastmod ,ZANNOTATIONSELECTEDTEXT,ZANNOTATIONASSETID from ZAEANNOTATION";
            select_HSQL = "select datetime((ZANNOTATIONCREATIONDATE) + 978307200, 'unixepoch', 'localtime') as lastmod ,ZANNOTATIONSELECTEDTEXT, ZANNOTATIONNOTE,ZANNOTATIONASSETID from ZAEANNOTATION where ZANNOTATIONTYPE = 2 and ZAEANNOTATION.ZANNOTATIONDELETED = 0";


            // Define the ADO.NET objects.
            SQLiteConnection con1 = new SQLiteConnection();
            //   ConfigurationManager.ConnectionStrings["bookinfo_conn"].ConnectionString = "|DataDirectory|"+Session["uid"].ToString()+"iBooks_v10252011_2152.sqlite";
         //   String con1new = "|DataDirectory|" + Session["uid"].ToString();
            //   AppDomain.CurrentDomain.SetData("DataDirectory", con1new);
            con1.ConnectionString = con1user ;    SQLiteCommand cmd_books = new SQLiteCommand(select_BSQL, con1);
            SQLiteDataReader reader1, reader2;
           

            SQLiteConnection con2 = new SQLiteConnection();
          //  String con2new = "|DataDirectory|" + Session["uid"].ToString();
            // ConfigurationSettings.AppSettings.AllKeys
            //    AppDomain.CurrentDomain.SetData("DataDirectory", con2new);
            con2.ConnectionString = con2user;
               SQLiteCommand cmd_hl = new SQLiteCommand(select_HSQL, con2);


            // Try to open database and read information.
            try
            {
                con1.Open();
                reader1 = cmd_books.ExecuteReader();
                con2.Open();
                reader2 = cmd_hl.ExecuteReader();
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
                DataTable dt1 = new DataTable();
                dt1.Load(reader1);
                DataTable dt2 = new DataTable();
                dt2.Load(reader2);

                IEnumerable<record> rquery = from row in dt1.AsEnumerable()
                                             join row2 in dt2.AsEnumerable()
                                            on row.Field<String>("ZDATABASEKEY") equals row2.Field<String>("ZANNOTATIONASSETID")
                                             //  group by row.Field<String>("ZBOOKTITLE")
                                             orderby row.Field<string>("ZBOOKTITLE"), row2.Field<String>("lastmod")
                                             select new record
                                             {
                                                 NAME = row.Field<string>("ZBOOKTITLE"),
                                                 Author = row.Field<string>("ZBOOKAUTHOR"),
                                                 //Chapter = row.Field<string>("ZNAME"),
                                                 //HIGHLIGHTCOLOR = (row.Field<Int64>("ZHIGHLIGHTCOLOR")).ToString(),
                                                 TEXTUALCONTEXT = row2.Field<string>("ZANNOTATIONSELECTEDTEXT"),
                                                 NOTES = row2.Field<string>("ZANNOTATIONNOTE"),
                                                 Date = row2.Field<string>("lastmod")
                                             };

                //group table by new { column1 = table["Column1"], column2 = table["Column2"] }
                //    into groupedTable
                //    select new
                //    {
                //        x = groupedTable.Key,  // Each Key contains column1 and column2
                //        y = groupedTable.Count()
                //    };
                //  GridView1.DataSource = rquery;
                // GridView1.DataBind();

                Gen_HTML(rquery);
            }

            catch
            {

            }

            finally
            {
                con1.Close();
                con2.Close();
            }
        }

      public void Gen_HTML(IEnumerable<record> rquery)
        {
            String bname = "";
            Boolean Bisnew = false;
            try
            {
                foreach (var a in rquery)
                {

                    // Label1.Text = Label1.Text+"<br/>"+a.NAME;
                    Bisnew = (bname == a.NAME) ? false : true;
                    bname = a.NAME;
                    //  Disnew = (Dat == a.Date) ? false : true;
                    // Dat = a.Date;

                    if (Bisnew)
                    {
                        if (a.TEXTUALCONTEXT != null)
                            BookData = BookData + String.Format("</ol><br/><br/><span style=\"font-size:9pt;\"><Strong><h1>{0} - by {1}</h1></strong></span><ol>", a.NAME, a.Author);
                        // Label1.Text = Label1.Text + String.Format("<br/>{0}",a.TEXTUALCONTEXT);
                    }
                    //if (Disnew)
                    //{
                    //    Label1.Text = Label1.Text + String.Format("</ol><br/>{0}<ol>", a.Date);
                    //    // Label1.Text = Label1.Text + String.Format("<br/>{0}",a.TEXTUALCONTEXT);
                    //}
                    if (a.TEXTUALCONTEXT != null)
                        BookData = BookData + String.Format("<li><span style=\"font:Verdana\">{0}</span> - <span style=\"font-style:italic\">(Highlighted on : {1})</span></li>", a.TEXTUALCONTEXT, a.Date);
               
                    if(a.NOTES!=null)
                        BookData = BookData + String.Format("<li><span style=\"font:Verdana\">{0}</span> - <span style=\"font-style:italic\">(Note created on : {1})</span></li>", a.NOTES, a.Date);
               
                }
                DirectoryInfo r = new DirectoryInfo(Server.MapPath("~") + "//output");
                r.CreateSubdirectory("" + Session["uid"]);


                // create a writer and open the file
                TextWriter tw = new StreamWriter(Server.MapPath("~") + "//output//" + Session["uid"] + "//HighlightsAll.html");

                // write a line of text to the file
                tw.WriteLine(BookData);

                // close the stream
                tw.Close();

                //generate EPUB
                HTML_to_EPUB();
          //Label1.Text += Server.MapPath("~")+"1";
            }
            //     reader1.Close();

                //GridView2.DataSource = rquery;
            //GridView2.DataBind();
            //    reader2.Close();
            catch (Exception err)
            {
                Label2.Text = "Error reading list of names. ";
                Label2.Text += err.Message;
            }

        }

        public void HTML_to_EPUB()
        {
            //1           string Args = @" C:\\temp\\Hig.html C:\\temp\\Hig.epub --output-profile ipad --level1-toc //h:h1";
            //2           System.Diagnostics.Process tfs = System.Diagnostics.Process.Start(Server.MapPath("~") + "epub_create\\ebook-convert.exe", Args);
            //3         tfs.WaitForExit();
        //    String pathexe = "C:\\inetpub\\wwwroot\\Rawprograms";
        //    System.Diagnostics.Process p = new System.Diagnostics.Process();
        ////    p.StartInfo.Arguments = string.Format("{0} {1}", pathexe + "\\epub_create\\HighlightsAll.html", pathexe+ "\\epub_create\\Highlights.epub");
        //    p.StartInfo.Arguments = string.Format("{0} {1}", ".\\HighlightsAll.html", ".\\Highlights.epub");
             
        //    p.StartInfo.WorkingDirectory = Path.Combine(pathexe, "epub_create");
        //    p.StartInfo.FileName = "ebook-convert.exe";
        //    p.StartInfo.UseShellExecute = false;
        //    p.StartInfo.CreateNoWindow = false;
        //    p.StartInfo.RedirectStandardInput = true;
        //    p.StartInfo.RedirectStandardOutput = true;
        //    p.StartInfo.RedirectStandardError = true;
     
         
        //    p.Start();
        //    string output = p.StandardOutput.ReadToEnd();
        //       Response.Write(output);
        //       p.Close();

            System.Diagnostics.Process si = new System.Diagnostics.Process();
         //   si.StartInfo.WorkingDirectory = Server.MapPath("~") + "\\epub_create";

            si.StartInfo.FileName = Server.MapPath("~") + "\\epub_create\\ebook-convert.exe";
            si.StartInfo.Arguments = @" " + Server.MapPath("~") + "\\output\\" + Session["uid"] + "\\HighlightsAll.html " + Server.MapPath("~") + "\\output\\" + Session["uid"] + "\\MyHighlights.epub --output-profile ipad --level1-toc //h:h1 --cover " + Server.MapPath("~") + "\\epub_create\\cover.jpg --authors Parthiv_dave --title Extracted_highlights";
           // si.StartInfo.Arguments = @" .\\HighlightsAll.html .\\Highlights.epub --output-profile ipad --level1-toc //h:h1";
            si.StartInfo.UseShellExecute = false;
            si.StartInfo.CreateNoWindow = true;
            si.StartInfo.RedirectStandardInput = true;
            si.StartInfo.RedirectStandardOutput = true;
            si.StartInfo.RedirectStandardError = true;
            si.Start();
            StreamWriter sIn = si.StandardInput;
            StreamReader sOut = si.StandardOutput;
            StreamReader sErr = si.StandardError;

            //     sIn.AutoFlush = true;
            //     sIn.Write(" HighlightsAll.html MyHighlights.epub --output-profile ipad --level1-toc //h:h1");
            string output = si.StandardOutput.ReadToEnd();
          //  Response.Write(output);
            si.Close();
            sIn.Close();
            sOut.Close();
            sErr.Close();
   
        
            Read_highlights();
        }


        private void Read_highlights()
        {
            // create reader & open file

            TextReader tr = new StreamReader(Server.MapPath("~") + "//output//"+Session["uid"]+"//HighlightsAll.html");

            // read a line of text
            Label1.Text = tr.ReadToEnd();

            // close the stream
            tr.Close();
            Session.Clear();
        }

       

        }
    }
