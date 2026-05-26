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

            // [4주차 R 기능 추가]
            // 1. DB에 과목 표 데이터 요청
            DataTable subjectData = db.GetSubjectList();

            // 2. 좌측 DataGridView의 데이터 원천(DataSource)에 연동
            dgvTasks.DataSource = subjectData;

            // [DataGridView 다크모드 스타일링
            dgvTasks.EnableHeadersVisualStyles = false; // Windows 기본 Header Style 해제 (Custom 허용)

            // 1. Header Style(제목칸 설정)
            dgvTasks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40); // 짙은 회색
            dgvTasks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // 흰색 글자
            dgvTasks.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(40, 40, 40);

            // 2. Cell Style(내용 스타일 설정)
            dgvTasks.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32); // 배경색(어둡게)
            dgvTasks.DefaultCellStyle.ForeColor = Color.FromArgb(248, 250, 252); // 하얀 글씨(부드럽게)

            // 3. 마우스로 셀로 선택했을 때(Neon Green -> 포인트 컬러)
            dgvTasks.DefaultCellStyle.SelectionBackColor = Color.FromArgb(163, 230, 53); // 네온 그린 배경
            dgvTasks.DefaultCellStyle.SelectionForeColor = Color.Black; // 선택된 글자는 검은색으로 선명하게

            // 4. 기타 테두리 및 그리드 선 스타일
            dgvTasks.GridColor = Color.FromArgb(50, 50, 50); // 그물망 선도 어둡게
            dgvTasks.RowHeadersVisible = false; // 맨 왼쪽 여백 화살표 칸 제거 (깔끔함 극대화)
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

                // 💡 [ 4주차 'U' 기능 결합 ] Timer 완료 시점에 DB에 25분 누적
                DatabaseManager dbManager = new DatabaseManager();
                bool updateResult = dbManager.UpdateSubjectTime(currentTask.Title, 25);

                if (updateResult)
                {
                    Console.WriteLine($"[시스템] '{currentTask.Title}' 과목에 25분이 정상 누적되었습니다.");
                }

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

        private void dgvTasks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 클릭한 줄의 인덱스가 정상적인 범위인지 체크 (헤더 클릭 방지 예외처리)
            if (e.RowIndex >= 0)
            {
                // 선택한 행(Row)을 가져옵니다.
                DataGridViewRow row = dgvTasks.Rows[e.RowIndex];

                // 표 칸에 적혀있는 값을 가져와서 currentTask 객체에 주입합니다.
                currentTask.Title = row.Cells["과목명"].Value.ToString();

                // 💡 나중에 UPDATE 쿼리를 날릴 때 사용하기 위해 ID 값도 저장해둡니다.
                // 임시로 currentTask 객체에 넘기거나 전역 변수에 ID를 보관합니다.
                // 여기서는 대기 중인 과목 이름을 상단 타이틀바에 실시간 반영합니다.
                this.Text = $"진행 중: {currentTask.Title}";

                MessageBox.Show($"['{currentTask.Title}'] 과목이 선택되었습니다. " +
                                $"몰입 시작 버튼을 누르면 시간이 누적됩니다.", "과목 선택 완료");
            }
        }
    }
}
