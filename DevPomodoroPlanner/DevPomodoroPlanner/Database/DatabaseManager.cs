using System;
using System.Data;
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

        // DB 데이터를 가상 표(DataTable) 형태로 긁어오는 메서드
        public DataTable GetSubjectList()
        {
            // SQL 쿼리문: AS 문법을 활용 한글 컬럼명으로 명시 -> 한국어 사용자를 위해
            string query = "SELECT subject_id AS '번호', " +
                               "subject_name AS '과목명', " +
                               "category AS '분류' FROM subject_table;";

            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // DB에서 가져온 데이터를 C# 표에 채워주는 전용 메서드(Adapter)
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // adapter가 가져온 데이터를 dt에 채우기
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB Select Error] 과목 로드 실패: {ex.Message}");
            }

            return dt; // 데이터를 호출한 곳으로 반환
        }

        // ⭐ 특정 과목을 몰입 시간을 기존 시간에 추가(누적)하는 메서드
        public bool UpdateSubjectTime(string subjectName, int minuteToAdd)
        {
            // 💡 기존 학습 시간에 새 시간을 더해주는(UPDATE ... + @min) 핵심 SQL 쿼리
            // 실제 설계에 따라 필드명은 유연하게 조절됩니다.
            // 여기서는 기획안의 subject_table 구조를 기반으로 쿼리를 칩니다.
            string query = "UPDATE subject_table " +
                           "SET total_study_time = total_study_time + @minutes " +
                           "WHERE subject_name = @name;";

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@minutes", minuteToAdd);
                        cmd.Parameters.AddWithValue("@name", subjectName);

                        cmd.ExecuteNonQuery(); // DB Data 갱신
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB Update Error] 시간 누적 실패: {ex.Message}");
                return false;
            }
        }

        // chart 시각화를 위해 과목명과 총 공부시간만 쏙 가져오는 메서드
        public DataTable GetChartData()
        {
            string query = "SELECT subject_name, total_study_time FROM subject_table;";
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                /* Console.WriteLine($"[DB Chart Error] 차트 데이터 로드 실패: {ex.Message}"); */
                System.Windows.Forms.MessageBox.Show(
                    $"[Chart DB Error]\n{ex.Message}\n\n과목 표는 절 나오는데 왜 Error가 날까요?", 
                    "Back-end 배달 사고 발생");
            }

            return dt;
        }
    }
}
