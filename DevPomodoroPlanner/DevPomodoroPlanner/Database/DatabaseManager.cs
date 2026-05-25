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

        // DB에 로그를 안전하게 삽입하는 메서드
        public bool InsertDevLog(string logContent, string errorCode)
        {
            // SQL 인젝션 공격을 방어하기 위해 매개변수(@) 구조를 사용합니다.
            string query = "INSERT INTO dev_log_table (log_content, error_code) VALUES (@content, @code);";

            try
            {
                // using문을 쓰면 DB 연결 후 컴퓨터 메모리가 알아서 깔끔하게 청소됩니다.
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open(); // DB 실행

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // @Content와 @code 자리에 진짜 사용자가 입력한 글자를 매핑해줍니다.
                        cmd.Parameters.AddWithValue("@Content", logContent);
                        cmd.Parameters.AddWithValue("@code", errorCode);

                        cmd.ExecuteNonQuery(); // 쿼리문 실행 및 저장
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB Insert Error] 로그 저장 실패: {ex.Message}");
                return false;
            }
        }
    }
}
