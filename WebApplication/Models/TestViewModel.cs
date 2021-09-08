using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestApplication.Models
{
    public class BaseModel
    {
        // 오류 메세지를 포함 하기 위한 BaseModel
        public string ErrorMessage { get; set; }
    }
        
    public class TestViewModel : BaseModel
    {
        // 분석할 URL 
        [Required]
        [Display(Name = "URL")]
        [Url]
        public string URL { get; set; }

        // 분석 방법 선택
        // PART : HTML 태그 제외
        // ALL : Text 전체
        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; }

        // 출력 묶음 단위
        [Required]
        [Display(Name = "출력 단위 묶음")]
        [Range(1, int.MaxValue, ErrorMessage = "1 부터 입력 가능합니다.")]
        public int BundleCount { get; set; }

        // 분석 결과 몫 
        [Display(Name = "몫")]
        public string Quota { get; set; }
        
        // 분석 결과 나머지
        [Display(Name = "나머지")]
        public string Rest { get; set; }
    }
}