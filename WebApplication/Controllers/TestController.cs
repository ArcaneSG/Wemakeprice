using CustomExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TestApplication.Models;

namespace WebApplication1.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public ActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Test(TestViewModel model)
        {
            // 모델 유효성 체크
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string strHtml = string.Empty;

            #region 1. 모든 문자 입력 가능

            try
            {
                // 입력 받은 URL 요청
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(model.URL);

                // 응답 받기
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // 웹사이트 별 인코딩 방식에 맞게 인코딩 하기 위해 선언
                Encoding encode = Encoding.GetEncoding(response.CharacterSet);

                // 응답받은 스트림을 사이트에서 사용하고 있는 인코딩 타입으로 인코딩
                StreamReader reader = new StreamReader(response.GetResponseStream(), encode);

                // 모든 문자 읽기
                strHtml = reader.ReadToEnd();

                reader.Close();

                response.Close();
            }
            catch (WebException webExcp)
            {
                model.ErrorMessage = webExcp.ToString();
                return View(model);
            }
            catch (Exception e)
            {
                model.ErrorMessage = e.Message.ToString();
                return View(model);
            }

            #endregion
            
            #region 2. 영어 및 숫자만 출력 & 3. 오름차순 출력

            // HTML 제거 옵션을 선택했을 경우 HTML TAG  제거
            if (model.Type == "PART") { strHtml = Regex.Replace(strHtml, @"<(.|\n)*?>", string.Empty); }
            
            // 교차 출력을 위해 숫자만 찾고 오름차순 정렬
            IEnumerator<char> NumberEnum = Regex.Replace(strHtml, @"[^0-9]", string.Empty).ToCharArray().OrderBy(n => n).GetEnumerator();
            
            // 교차 출력을 위해 영어만 찾고 오름차순 정렬
            // 기본 정렬로는 처리 불가 하여 확장 메서드를 활용하여 처리
            // 함수로도 처리 가능 하지만 재사용을 많이 한다는 가정 하에 확장 메서드를 활용해 보았습니다.
            IEnumerator<char> EnglishEnum = Regex.Replace(strHtml, @"[^a-zA-Z]", string.Empty).ToCharArray().OrderBy(e => e.CustomASCII()).GetEnumerator();

            #region LINQ 스타일

            /*
            IEnumerator<char> NumberEnumLinq = (from num in Regex.Replace(strHtml, @"[^0-9]", string.Empty).ToCharArray()
                                                orderby num
                                                select num).GetEnumerator();

            IEnumerator<char> EnglishEnumLinq = (from eng in Regex.Replace(strHtml, @"[^a-zA-Z]", string.Empty)
                                             orderby eng.CustomASCII()
                                             select eng).GetEnumerator();
            */

            #endregion

            #endregion

            #region 4. 영어 숫자 Mix

            bool hasEnglish;
            bool hasNumber;

            // Enumerator 를 사용하여 끝요소 까지 출력
            while ((hasEnglish = EnglishEnum.MoveNext()) | (hasNumber = NumberEnum.MoveNext()))
            {
                // 교차 출력 시 영문자 먼저 출력
                model.Quota += (hasEnglish ? EnglishEnum.Current.ToString() : string.Empty) + (hasNumber ? NumberEnum.Current.ToString() : string.Empty);
            }

            #endregion

            #region 5. 출력 묶음 단위

            int resultLen = model.Quota.Length;

            // 페이지에 텍스트가 존재 할 경우에만 작업
            if(resultLen > 0)
            {
                // 출력 단위 묶음을 통해 몫과 나머지 계산
                int calQuota = resultLen / model.BundleCount;
                int calRest = resultLen % model.BundleCount;

                // 나머지가 존재 할 경우에만 작업
                if (calRest > 0)
                {
                    model.Rest = model.Quota.Substring(resultLen - calRest);
                    model.Quota = model.Quota.Remove(calQuota * model.BundleCount);
                }

                // 출력페이지에 텍스트 표현을 위해 추가
                ViewBag.calQuota = (calQuota * model.BundleCount).ToString();
                ViewBag.calRest = calRest.ToString();
            }

            #endregion
            
            return View(model);
        }
    }
}

namespace CustomExtensions
{
    public static class CharExtension
    {
        // 영문자 알파벳 순 정렬을 위해 사용
        public static float CustomASCII(this char input)
        {
            float result = Convert.ToInt32(input);

            // 영어 소문자를 영어 대문자 뒤로 정렬 시키기 위해 아스키 코드 활용
            // 아스키 코드 값 - A : 65, a : 97
            // 커스텀 코드 값 - A : 65, a : 65.5
            if (result > 96 && result < 123)
            {
                result = Convert.ToSingle(result - 32 + 0.5);
            }
            
            return result;
        }
    }
}
