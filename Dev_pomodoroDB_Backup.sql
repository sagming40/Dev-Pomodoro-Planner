-- --------------------------------------------------------
-- 호스트:                          127.0.0.1
-- 서버 버전:                        12.2.2-MariaDB - MariaDB Server
-- 서버 OS:                        Win64
-- HeidiSQL 버전:                  12.14.0.7165
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- dev_pomodoro_db 데이터베이스 구조 내보내기
CREATE DATABASE IF NOT EXISTS `dev_pomodoro_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_uca1400_ai_ci */;
USE `dev_pomodoro_db`;

-- 테이블 dev_pomodoro_db.dev_log_table 구조 내보내기
CREATE TABLE IF NOT EXISTS `dev_log_table` (
  `log_id` int(11) NOT NULL AUTO_INCREMENT,
  `log_content` text NOT NULL,
  `error_code` varchar(100) DEFAULT NULL,
  `created_datatime` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`log_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- 내보낼 데이터가 선택되어 있지 않습니다.

-- 테이블 dev_pomodoro_db.subject_table 구조 내보내기
CREATE TABLE IF NOT EXISTS `subject_table` (
  `subject_id` int(11) NOT NULL AUTO_INCREMENT,
  `subject_name` varchar(50) NOT NULL,
  `category` varchar(50) DEFAULT NULL,
  `total_study_time` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`subject_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- 내보낼 데이터가 선택되어 있지 않습니다.

-- 테이블 dev_pomodoro_db.todo_table 구조 내보내기
CREATE TABLE IF NOT EXISTS `todo_table` (
  `todo_id` int(11) NOT NULL AUTO_INCREMENT,
  `subject_id` int(11) DEFAULT NULL,
  `title` varchar(100) NOT NULL,
  `target_minutes` int(11) DEFAULT 25,
  `actual_minutes` int(11) DEFAULT 0,
  `created_date` date DEFAULT NULL,
  PRIMARY KEY (`todo_id`),
  KEY `subject_id` (`subject_id`),
  CONSTRAINT `1` FOREIGN KEY (`subject_id`) REFERENCES `subject_table` (`subject_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- 내보낼 데이터가 선택되어 있지 않습니다.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
