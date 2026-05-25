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
        // 전역 변수: 프로그램 전체에서 공유되는 데이터들
        private int timeLeft; // 남은 시간 (초 단위로 계산)
        private Models.StudyTask currentTask; // 현재 진행 중인 공부 과제 객체

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

        private void tmrPomodoro_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                // 1초씩 깎음
                timeLeft--;

                // 화면 갱신 (시간 표시 및 ProgressBar)
                UpdateTimerDisplay();
            }
        }

        // 시간을 예쁘게 00:00 형태로 바꿔주는 도우미 메서드 (문자열 보간 사용)
        private void UpdateTimerDisplay()
        {
            int minutes = timeLeft / 60;
            int seconds = timeLeft % 60;

            // lblTimer 텍스트 갱신 (예: 25:00)
            lblTimer.Text = $"{minutes:D2}:{seconds:D2}";

            // pbProgress 진행바 갱신 (남은 시간에 비례해서 줄어듦)
            // 최대값을 1500초(25분)로 가정했을 때의 예시
            pbProgress.Value = timeLeft;
        }
    }
}
