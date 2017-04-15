using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace SSConnecEx
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //create database sample
            //go
            //use sample
            //go
            //create table products (id int, name varchar(100), price int)
            //go
            //insert into products values(1, 'racecare', 10)
            //insert into products values (2, 'balloons', 110)
            //insert into products values (3, 'webcam', 12)
            //go
            //select id,name,price from products

            // 1.) Inital
            SqlConnection con = new SqlConnection("data source=CATROOM\\SQLEXPRESS; database=sample; integrated security=SSPI");
            try
            {
                SqlCommand cmd = new SqlCommand("select id,name,price from products", con);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                GridView1.DataSource = sdr;
                GridView1.DataBind();
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }


            //2.) using the using command
            string CS2 = "data source=CATROOM\\SQLEXPRESS; database=sample; integrated security=SSPI";
            using (SqlConnection con2 = new SqlConnection(CS2))
            {
                SqlCommand cmd2 = new SqlCommand("select id,name,price from products", con2);
                con2.Open();
                SqlDataReader sdr = cmd2.ExecuteReader();
                GridView2.DataSource = sdr;
                GridView2.DataBind();
            }

            //3.) connection string in app config
            string CS3=ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            //Can also call by index (bad practice)
            //ConfigurationManager.ConnectionStrings[0];
            using (SqlConnection con3 = new SqlConnection(CS3))
            {
                SqlCommand cmd3 = new SqlCommand("select id,name,price from products", con3);
                con3.Open();
                SqlDataReader sdr = cmd3.ExecuteReader();
                GridView3.DataSource = sdr;
                GridView3.DataBind();
            }


            //4.) SQL command class
            
            // ExecuteReader- use when tsql retunrs multiple rows
            // ExecuteNonquery- use when perform Insert/Upd/Del
            // ExeculateScalar- when returns single scalar value

            string CS4 = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            //ExecuteReader
            using (SqlConnection con4 = new SqlConnection(CS4))
            {
                SqlCommand cmd4 = new SqlCommand("select id,name,price from products", con4);
                con4.Open();
                SqlDataReader sdr = cmd4.ExecuteReader();
                GridView4.DataSource = sdr;
                GridView4.DataBind();
            }

            // ExeculateScalar (return single value)
            using (SqlConnection con5 = new SqlConnection(CS4))
            {
                SqlCommand cmd5 = new SqlCommand("select count(*) from products", con5);
                con5.Open();
                int count=(int)cmd5.ExecuteScalar();
                Response.Write("Using ExecuteScalar total rows count= " + count.ToString());
                Response.Write("<br>");
            }

            // ExeculateNonQuery (for CRUD)
            using (SqlConnection con6 = new SqlConnection(CS4))
            {
                con6.Open();

                SqlCommand cmd6 = new SqlCommand("delete from products where id=4", con6);
                int returnValueOfRowsChanged = cmd6.ExecuteNonQuery();

                // reuse cmd object
                cmd6 = new SqlCommand("insert into products values(4, 'boop', 121)", con6);
                returnValueOfRowsChanged =cmd6.ExecuteNonQuery();
                Response.Write("Using ExeculateNonQuery total rows inserted= " + returnValueOfRowsChanged.ToString());
                Response.Write("<br>");
            }

//create procedure spListProducts
//@id int
//as
//begin
//select* from products
//where id = @id
//end

            //5.) Call  stored procedure (help prevent sql injection)
            using (SqlConnection con7 = new SqlConnection(CS4))
            {
                SqlCommand cmd7 = new SqlCommand("spListProducts", con7);
                cmd7.CommandType = System.Data.CommandType.StoredProcedure;
                cmd7.Parameters.AddWithValue("@id", 3);
                con7.Open();
                SqlDataReader sdr = cmd7.ExecuteReader();
                GridView5.DataSource = sdr;
                GridView5.DataBind();
            }


//create procedure spListProductsOutput
//@id int,
//@count int out
//as
//begin
//select @count = count(*) from products where id = @id
//end

            //5.) Call  stored procedure with output parameters
            using (SqlConnection con8 = new SqlConnection(CS4))
            {
                SqlCommand cmd8 = new SqlCommand("spListProductsOutput", con8);
                cmd8.CommandType = System.Data.CommandType.StoredProcedure;
                cmd8.Parameters.AddWithValue("@id", 3);


                SqlParameter outputParameter = new SqlParameter();
                outputParameter.ParameterName = "@count";
                outputParameter.SqlDbType = System.Data.SqlDbType.Int;
                outputParameter.Direction = System.Data.ParameterDirection.Output;
                cmd8.Parameters.Add(outputParameter);
                
                con8.Open();
                SqlDataReader sdr = cmd8.ExecuteReader();

                con.Open();

                int returnOutputValue = (int)outputParameter.Value;
                Response.Write("Output parameter value= " + returnOutputValue.ToString());
                Response.Write("<br>");

            }



            //6.) SQLDataReader
            using (SqlConnection con9 = new SqlConnection(CS4))
            {
                SqlCommand cmd9 = new SqlCommand("select id,name,price from products", con9);
                con9.Open();

                using (SqlDataReader sdr = cmd9.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Columns.Add("id");
                    table.Columns.Add("name");
                    table.Columns.Add("price");
                    table.Columns.Add("discountprice");

                    while (sdr.Read())
                    {
                        DataRow datarow = table.NewRow();
                        int originalPrice = (int)sdr["price"];
                        double discountedPrice = originalPrice * .9;

                        datarow["id"] = sdr["ID"];
                        datarow["name"] = sdr["name"];
                        datarow["price"] = originalPrice;
                        datarow["discountprice"] = discountedPrice;
                        table.Rows.Add(datarow);
                    }
                    GridView6.DataSource = table;
                    GridView6.DataBind();
                }
            }


            //7.) SQLDataReader NextResult
            using (SqlConnection con10 = new SqlConnection(CS4))
            {
                SqlCommand cmd10 = new SqlCommand("select id,price from products;select id,name from products", con10);
                con10.Open();
                using (SqlDataReader sdr = cmd10.ExecuteReader())
                {
                    GridView7.DataSource = sdr;
                    GridView7.DataBind();

                    while (sdr.NextResult())
                    {
                        GridView8.DataSource = sdr;
                        GridView8.DataBind();
                    }

                }
            }


            //8.) SQLDataApater and DataSet
            using (SqlConnection con10 = new SqlConnection(CS4))
            {
                SqlDataAdapter da = new SqlDataAdapter("select id,price,name from products", con10);
                DataSet ds = new DataSet(); //in memory representation of table in memory
                da.Fill(ds);

                GridView9.DataSource = ds;
                GridView9.DataBind();

            }


    //create procedure spListTwoResults
    //as
    //begin
    //select* from products
    //select id from products
    //end

            //9.) DataSet in asp.net
            using (SqlConnection con11 = new SqlConnection(CS4))
            {
                SqlDataAdapter da = new SqlDataAdapter("spListTwoResults", con11);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                DataSet ds = new DataSet(); //in memory representation of table in memory
                da.Fill(ds);

                GridView10.DataSource = ds.Tables[0];
                GridView10.DataBind();

                GridView11.DataSource = ds.Tables[1];
                GridView11.DataBind();
            }


            //10.) Caching dataset
            using (SqlConnection con11 = new SqlConnection(CS4))
            {
                SqlDataAdapter da = new SqlDataAdapter("spListTwoResults", con11);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                DataSet ds = new DataSet(); //in memory representation of table in memory
                da.Fill(ds);

                GridView10.DataSource = ds.Tables[0];
                GridView10.DataBind();

                GridView11.DataSource = ds.Tables[1];
                GridView11.DataBind();
            }


//Create Table tblStudents
//(
// ID int identity primary key,
// Name nvarchar(50),
// Gender nvarchar(20),
// TotalMarks int
//)

//Insert into tblStudents values('Mark Hastings', 'Male', 900)
//Insert into tblStudents values('Pam Nicholas', 'Female', 760)
//Insert into tblStudents values('John Stenson', 'Male', 980)
//Insert into tblStudents values('Ram Gerald', 'Male', 990)
//Insert into tblStudents values('Ron Simpson', 'Male', 440)
//Insert into tblStudents values('Able Wicht', 'Male', 320)
//Insert into tblStudents values('Steve Thompson', 'Male', 983)
//Insert into tblStudents values('James Bynes', 'Male', 720)
//Insert into tblStudents values('Mary Ward', 'Female', 870)
//Insert into tblStudents values('Nick Niron', 'Male', 680)

            //11.) CommandBuilder
            using (SqlConnection con11 = new SqlConnection(CS4))
            {
                SqlDataAdapter da = new SqlDataAdapter("spListTwoResults", con11);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                DataSet ds = new DataSet(); //in memory representation of table in memory
                da.Fill(ds);

                GridView10.DataSource = ds.Tables[0];
                GridView10.DataBind();

                GridView11.DataSource = ds.Tables[1];
                GridView11.DataBind();
            }




        }//end pageload

        //10.) Caching dataset
        protected void btnLoadData_Click(object sender, EventArgs e)
        {
            if(Cache["Data"]==null)
            {
                string CS4 = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                using (SqlConnection con12 = new SqlConnection(CS4))
                {
                    SqlDataAdapter da = new SqlDataAdapter("select id,price,name from products", con12);
                    DataSet ds = new DataSet(); //in memory representation of table in memory
                    da.Fill(ds);

                    Cache["Data"] = ds;

                    GridView12.DataSource = (DataSet)Cache["Data"];
                    GridView12.DataBind();
                    lblMessage.Text = "Loaded from DB";
                }
            }
            else
            {
                GridView12.DataSource = (DataSet)Cache["Data"];
                GridView12.DataBind();
                lblMessage.Text = "Loaded from Cache";
            }

        }

        //10.) Caching dataset
        protected void btnClearCache_Click(object sender, EventArgs e)
        {
            if (Cache["Data"] != null)
            {
                Cache.Remove("Data");
                lblMessage.Text = "Removed from Cache";
            }
            else
            {
                lblMessage.Text = "Not in Cache";
            }
        }

        //11.) CommandBuilder Generates insert updates delets based of of select command
        protected void btnGetStudent_Click(object sender, EventArgs e)
        {
            string CS4 = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            SqlConnection con13 = new SqlConnection(CS4);
            string squery = "select * from tblStudents where id = "+txtStudentID.Text;
            SqlDataAdapter da = new SqlDataAdapter(squery, con13);
            DataSet ds = new DataSet(); //in memory representation of table in memory
            da.Fill(ds,"Students");

            ViewState["SQL_Query"] = squery;
            ViewState["DataSet"] = ds;

            if (ds.Tables["Students"].Rows.Count>0)
            {
                DataRow dr = ds.Tables["Students"].Rows[0];
                txtStudentName.Text = dr["Name"].ToString();
                txtTotalMarks.Text = dr["TotalMarks"].ToString();
                ddlGender.SelectedValue= dr["Gender"].ToString();
            }
            else
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "No student record with id " + txtStudentID.Text;
            }
        }

        //11.) CommandBuilder Generates insert updates delets based of of select command
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string CS4 = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            SqlConnection con13 = new SqlConnection(CS4);
            SqlDataAdapter da = new SqlDataAdapter((string)ViewState["SQL_Query"], con13);

            SqlCommandBuilder builder = new SqlCommandBuilder(da);

            DataSet ds = (DataSet)ViewState["DataSet"];

            if (ds.Tables["Students"].Rows.Count > 0)
            {
                DataRow dr = ds.Tables["Students"].Rows[0];
                dr["Name"]= txtStudentName.Text;
                dr["TotalMarks"]= txtTotalMarks.Text;
                dr["Gender"] = ddlGender.SelectedValue;
            }

            int rowsUpdated = da.Update(ds, "Students");

            if (rowsUpdated > 0)
            {
                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = "Rows updated";
            }
            else
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "No rows updated";
            }
        }



    }




}
