using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector; // NuGet으로 설치한 라이브러리 로드

namespace DevPomodoroPlanner.Database
{
    public class DatabaseManager
    {
        // 접근 한정자 private으로 DB 접속 정보 완전 은닉 (캡슐화)
        // 외부 클래스(UI 등)에서 이 문자열을 직접 변경하거나 훔쳐볼 수 없음
        private string connectionString = "Server=localhost;Database=dev_pomodoro_db;Uid=root;Pwd=4040;";

        // 외부에서는 오직 이 public 메서드를 통해서만 안전하게 연결 객체를 받아갈 수 있음
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // [2주차] UI 개발 시 버튼으로 테스트 해볼 DB 연결 검증 매서드
        public bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open(); // DB 연결 시도
                    return true; // 성공 시 true 반환
                }
            }
            catch (Exception ex)
            {
                // 에러 코드 출력 -> 문자열 보간 활용
                Console.WriteLine($"[DB Error] 연결 실패: {ex.Message}");
                return false;
            }
        }
    }
}
