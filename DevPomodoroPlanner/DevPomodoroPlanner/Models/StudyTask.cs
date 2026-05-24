using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevPomodoroPlanner.Models
{
    // : PlannerItem 을 붙여 부모 클래스 상속
    public class StudyTask : PlannerItem
    { 
        // StudyTask만의 고유 속성 추가
        public int SubjectId { get; set; }
        public int TargetMinutes { get; set; }
        public int ActualMinutes { get; set; }

        // 부모가 물려준 추상 메서드를 override 키워드로 실제 구현
        // 문자열 보간($"{ }") 사용
        public override string GetTaskSummary()
        {
            return $"[{CreatedDate:yyyy-MM-dd}] {Title} - 목표: {TargetMinutes}분 / 실제 몰입: {ActualMinutes}분";
        }
    }
}
