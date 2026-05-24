using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevPomodoroPlanner.Models
{
    // abstract 키워드가 붙은 추상 클래스 (단독으로 객체 생성 불가, 오직 뼈대 역할)
    public abstract class PlannerItem
    {
        // 공통 속성 정의
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }

        // 자식 클래스들이 각자 사정에 맞게 반드시 구현(재정의)해야 하는 추상 메서드
        public abstract string GetTaskSummary();
    }
}
