using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace W12_THA_BRYAN_C
{
    public partial class Form1 : Form
    {
        MySqlConnection MySqlConnect;
        MySqlCommand sqlCommand;
        MySqlDataAdapter sqlAdapter;
        DataTable dtPlayer = new DataTable();
        DataTable dtTeamName = new DataTable();
        DataTable dtNationality = new DataTable();
        DataTable dtPosition = new DataTable();
        DataTable dtManager = new DataTable();
        string SqlQuery;
        MySqlDataReader sqlReader;

        public Form1()
        {
            string StringConnect = "server=localhost;uid=root;pwd=AniazoExodus12!;database=premier_league ; ";
            MySqlConnect = new MySqlConnection(StringConnect);

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            SqlQuery = "SELECT team_name, team_id FROM team;";
            sqlCommand = new MySqlCommand(SqlQuery, MySqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeamName);
            CBTeams.DataSource = dtTeamName;
            CBTeams.DisplayMember = "team_name";
            CBTeams.ValueMember = "team_id";

            dtTeamName = new DataTable();
            SqlQuery = "SELECT team_name, team_id FROM team;";
            sqlCommand = new MySqlCommand(SqlQuery, MySqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeamName);
            CBManager.DataSource = dtTeamName;
            CBManager.DisplayMember = "team_name";
            CBManager.ValueMember = "team_id";

            dtTeamName = new DataTable();
            SqlQuery = "SELECT team_name, team_id FROM team;";
            sqlCommand = new MySqlCommand(SqlQuery, MySqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeamName);
            CBRemovePlayer.DataSource = dtTeamName;
            CBRemovePlayer.DisplayMember = "team_name";
            CBRemovePlayer.ValueMember = "team_id";

            SqlQuery = "SELECT nation, nationality_id FROM nationality;";
            sqlCommand = new MySqlCommand(SqlQuery, MySqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtNationality);
            CBNationality.DataSource = dtNationality;
            CBNationality.DisplayMember = "nation";
            CBNationality.ValueMember = "nationality_id";

            SqlQuery = "SELECT playing_pos FROM player GROUP BY playing_pos;";
            sqlCommand = new MySqlCommand(SqlQuery, MySqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPosition);
            CBPosition.DataSource = dtPosition;
            CBPosition.ValueMember = "playing_pos";

        }
        private void buttonAddPlayer_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox_Height.Text) || !string.IsNullOrEmpty(textBox_ID.Text) || !string.IsNullOrEmpty(textBox_NamePlayer.Text) || !string.IsNullOrEmpty(textBox_NumberPlayer.Text)
               || !string.IsNullOrEmpty(textBox_Weight.Text))
            {
                string ID = textBox_ID.Text;
                string Name = textBox_NamePlayer.Text;
                string Height = textBox_Height.Text;
                string Weight = textBox_Weight.Text;
                string Number = textBox_NumberPlayer.Text;
                string MysQlCommand = $"insert into player value ('{ID}','{Number}','{Name}', '{CBNationality.SelectedValue}', '{CBPosition.SelectedValue}'," +
                    $"'{Height}', '{Weight}', '{dateTimePicker1.Text}', '{CBTeams.SelectedValue}', 1, 0)";
                try
                {
                    MySqlConnect.Open();
                    sqlCommand = new MySqlCommand(MysQlCommand, MySqlConnect);
                    sqlReader = sqlCommand.ExecuteReader();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    MySqlConnect.Close();
                }
            }
            else
            {
                MessageBox.Show("Fill The Blank");
            }
        }
        private void CBRemovePlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateDGVRemovePlayer();
        }

        private void updateDGVRemovePlayer()
        {
            dtPlayer = new DataTable();
            SqlQuery = "select p.player_id, p.team_number, p.player_name, n.nation, p.playing_pos, p.height, p.weight, p.birthdate, t.team_name FROM player p, nationality n, team t \r\nWHERE p.nationality_id = n.nationality_id AND t.team_id = p.team_id AND p.team_id = '" + CBRemovePlayer.SelectedValue + "' AND p.status = 1;";
            sqlCommand = new MySqlCommand(SqlQuery, MySqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPlayer);
            dataGridView3.DataSource = dtPlayer;
        }

        private void buttonRemovePlayer_Click(object sender, EventArgs e)
        {
            if (dataGridView3.Rows.Count > 12)
            {
                string id = dataGridView3.CurrentRow.Cells[0].Value.ToString();
                string MysQlCommand = $"update player set status = 0 where player_id = '{id}'";
                try
                {
                    MySqlConnect.Open();
                    sqlCommand = new MySqlCommand(MysQlCommand, MySqlConnect);
                    sqlReader = sqlCommand.ExecuteReader();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    MySqlConnect.Close();
                    updateDGVRemovePlayer();
                }
            }
            else
            {
                MessageBox.Show("Player Less than 11");
            }
        }

        private void DGVManagerChanged()
        {

            dtManager = new DataTable();
            SqlQuery = "SELECT m.manager_id, m.manager_name, n.nation AS nation, m.birthdate FROM manager m, team t, nationality n WHERE m.manager_id = t.manager_id\r\nAND m.nationality_id = n.nationality_id AND t.team_id = '" + CBManager.SelectedValue + "';";
            sqlCommand = new MySqlCommand(SqlQuery, MySqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtManager);
            dataGridView1.DataSource = dtManager;

            dtManager = new DataTable();
            SqlQuery = "SELECT manager_id, manager_name, n.nation, m.birthdate  FROM manager m, nationality n WHERE m.nationality_id = n.nationality_id AND m.working = 0;";
            sqlCommand = new MySqlCommand(SqlQuery, MySqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtManager);
            dataGridView2.DataSource = dtManager;
        }
        private void comboBox_Teams2_SelectedIndexChanged(object sender, EventArgs e)
        {
            DGVManagerChanged();
        }
        private void buttonChangeManager_Click(object sender, EventArgs e)
        {
            string id = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            string MysQlCommand = $"update team set manager_id = '" + id + "' WHERE team_id = '" + CBManager.SelectedValue + "';";
            try
            {
                MySqlConnect.Open();
                sqlCommand = new MySqlCommand(MysQlCommand, MySqlConnect);
                sqlReader = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                MySqlConnect.Close();
            }

            string id2 = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            string MysQlCommand2 = $"update manager set working = 0 WHERE manager_id = '" + id2 + "';";
            try
            {
                MySqlConnect.Open();
                sqlCommand = new MySqlCommand(MysQlCommand2, MySqlConnect);
                sqlReader = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                MySqlConnect.Close();
            }

            string MysQlCommand3 = $"update manager set working = 1 WHERE manager_id =  '" + id + "';";
            try
            {
                MySqlConnect.Open();
                sqlCommand = new MySqlCommand(MysQlCommand3, MySqlConnect);
                sqlReader = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                MySqlConnect.Close();
                DGVManagerChanged();
            }
        }

       
    }
}
