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

            // 1주 차에 만든 StudyTask 클래스 활용
            currentTask = new Models.StudyTask()
            {
                Title = "객체지향 프로그래밍",
                TargetMinutes = 25,
                CreatedDate = DateTime.Now
            };

            // 폼 타이틀에 현재 진행중인 과목 띄우기 (문자열 보간)
            this.Text = $"진행 중: {currentTask.Title}";
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

            else
            {
                // 시간이 다 됐을 때 (0초가 되었을 때) 타이머 멈춤
                tmrPomodoro.Stop();

                // 타이머가 끝나면 진행바와 시간 글자를 원래대로 리셋
                timeLeft = 0;
                lblTimer.Text = "25:00";
                pbProgress.Value = 0;

                // 버튼 상태 원래대로 복구
                btnStart.Enabled = true;
                btnStop.Enabled = false;

                // 💡 사용자가 자연스럽게 우측의 일지를 쓰도록 유도하는 메세지창
                MessageBox.Show("25분 간의 Pomodoro 몰입이 끝났습니다! 👏👏" +
                                "\n\n우측 에디터에 오늘 집중한 내용이나 해결한 에러 로그를 작성하고 [저장] 버튼을 눌러주세요", "몰입 종료");
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            // 테스트를 위해 25분(1500초)을 세팅합니다.
            if (timeLeft <= 0)
            {
                timeLeft = 1500;
                pbProgress.Maximum = 1500;
                pbProgress.Value = 1500;
            }

            tmrPomodoro.Start(); // 타이머 시작
            btnStart.Enabled = false; // 타이머가 시작되면 몰입 시작 버튼 비활성화
            btnStop.Enabled = true; // 일시 정지 버튼은 활성화
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            tmrPomodoro.Stop(); // 타이머 일시정지
            btnStart.Enabled = true; // 일시 정지 버튼을 클릭하면 몰입 시작 버튼 활성화
            btnStop.Enabled = false; // 일시 정지 버튼은 다시 비활성화
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            // 1. 예외 처리: 만약 사용자가 공백 상태로 저장을 눌렀을 때 출력 문구
            if (string.IsNullOrWhiteSpace(rtbDevLog.Text))
            {
                MessageBox.Show("저장할 내용이 없습니다. 오늘 배운 내용이나 에러 로그를 기록해 주세요.", "안내");
                return;
            }

            // 2. DB 매니저 객체 생성
            DatabaseManager db = new DatabaseManager();

            // 3. RichTextBox에 적힌 텍스트를 통째로 가져와 DB에 INSERT 요청
            // (테스트 단계이므로 에러코드는 "NONE"으로 임시 지정)
            bool isSuccess = db.InsertDevLog(rtbDevLog.Text, "NONE");

            // 4. 결과에 따른 피드백
            if (isSuccess)
            {
                MessageBox.Show("오늘의 몰입일지가 MariaDB에 안전하게 기록되었습니다! 🎉", "저장 성공");
                rtbDevLog.Clear(); // 다음 저장을 위해 텍스트 상자 비우기
            }
            else
            {
                MessageBox.Show("DB 저장에 실패했습니다. 코드를 다시 확인해 보세요. 😭", "저장 실패");
            }
        }
    }
}
