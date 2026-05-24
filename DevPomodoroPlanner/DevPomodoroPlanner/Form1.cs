using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevPomodoroPlanner.Database; // 💡 Database 폴더 연결

namespace DevPomodoroPlanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 화면이 켜질 때 자동으로 실행
        private void Form1_Load(object sender, EventArgs e)
        {
            // 1. DB 매니저 객체 생성
            DatabaseManager db = new DatabaseManager();

            // 2. 연결 테스트 실행 후 결과 팝업창에 출력
            if (db.TestConnection())
            {
                MessageBox.Show("DB 연결 성공. 1주차 개발 검증 완료", "Complete 🎉");
            }
            else
            {
                MessageBox.Show("DB 연결 실패... HeidiSQL이 켜져있는지, 혹은 Pwd를 확인하세요.", "실패 😭");
            }
        }
    }
}
