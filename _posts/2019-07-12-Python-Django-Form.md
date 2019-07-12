---
title: Django 04. Django 설문조사 앱 만들기
date: '2019-07-12 15:00:00'
categories: Python/Django
comments: true
---
이번 포스트에서는 Django로 설문조사 앱을 만들어 볼 것이다. 

(이 포스트는 [YOUTUBE : LIFE SOFT](https://www.youtube.com/channel/UCqRTjWqD-ZWHj0ZoPSKVWBw) 님이 게시하시는 글을 통해 공부하고 작성한 것이다.)

### 1. 새로운 프로젝트 만들기

**File -> New -> Other (Ctrl + N)**을 누르면 다양한 옵션들이 뜰 것이다.  
**Django**를 검색하면 나오는 **PyDev**의 **Django**를 선택하고, 이름을 정해서 만들어주자.   

### 2. 기본 테이블 생성 및 관리자 계정 (super user) 생성

<code> python manage.py migrate </code>  
**Anaconda Prompt**에서 해당 프로젝트의 디렉토리로 간 뒤 이 코드를 입력해서 **db.sqlite3**라는 기본 테이블을 해당 디렉토리에 생성하자.  
<code> python manage.py createsuperuser  </code>  
또한, 이 코드를 입력해서 관리자 계정을 만들어주자.

### 3. 설문조사 앱 만들기 및 세팅

**Anaconda prompt**에서 해당 프로젝트의 디렉토리에 다음 코드를 입력한다.  

<pre><code>python manage.py startapp survey </code></pre>
그러면 해당 디렉토리에 설문조사 앱에 관련된 디렉토리가 추가된다.  
이 작업 후에  **projname/settings.py**를 조금 수정해야한다.   
**INSTALLED_APPS**라는 항목 안에 **‘survey’,**를 추가해주고, **LANGUAGE_CODE**을 **‘ko-kr’**, **TIME_ZONE**을 **‘Asia/Seoul’**로 수정해준다.   
이전 포스팅에서 [디버깅 툴바](https://hwangtolee.github.io/python/django/2019/07/12/Python-Django-Debug-Toolbar.html)에 대해 다뤘는데, 이 포스팅에서부터는 디버깅 툴바를 사용할 것이다.  
디버깅 툴바에 대한 설정도 해주자.

우선 아래 코드를 추가해준다.

```python
INTERNAL_IPS = ('127.0.0.1',)
```

또한, **INSTALLED_APPS**라는 항목 안에 **‘debug_toolbar’,**를 추가해주고, **MIDDLEWARE**라는 항목 안에 **'debug_toolbar.middleware.DebugToolbarMiddleware'**,를 추가해준다.

### 4. 모델 클래스 정의와 Admin 사이트 설정

**<u>survey/models.py</u>**  

```python
from django.db import models

class Survey(models.Model):
    survey_idx = models.AutoField(primary_key=True)
    question = models.TextField(null=False)
    ans1 = models.TextField(null=True)
    ans2 = models.TextField(null=True)
    ans3 = models.TextField(null=True)
    ans4 = models.TextField(null=True)
    status = models.CharField(max_length=1, default="y")
    
class Answer(models.Model):
    answer_idx = models.AutoField(primary_key=True)
    survey_idx = models.IntegerField()
    num = models.IntegerField()
```

**django**에서 지원하는 **models**의 **Model**이라는 클래스를 상속하는 자식 클래스 **Survy**와 **Answer**를 만들어준다.  

**TextField**는 여러줄이 가능한 필드지만, **CharField**는 한줄만 가능하다. 

**<u>survey/admin.py</u>**  

```python
from django.contrib import admin
from survey.models import Survey, Answer

class SurveyAdmin(admin.ModelAdmin):
    list_display = ("question", "ans1", "ans2", "ans3", "ans4", "status")
    
admin.site.register(Survey, SurveyAdmin)
admin.site.register(Answer)
```

**SurveyAdmin** 클래스를 만들어서 **Survey**라는 클래스가 관리자페이지에서 어떻게 보일지 정의해준다.   
**admin.site.register()** 함수를 통해 **Survey**와 **SurveyAdmin**, **Answer** 클래스를 관리자 페이지에 등록해준다.  **register()** 함수는 2개까지만 한 번에 등록할 수 있기 때문에 3개 이상은 2개씩 나누어 써줘야 한다.

이제 이렇게 만든 모델 클래스들을 데이터베이스에 반영해준다.   

<pre><code>python manage.py makemigrations
python manage.py migrate</code>
</pre>

### 5.  웹서버 구동과 관리자 페이지

<code>python manage.py runserver localhost:80</code>

이 코드를 **anaconda** **prompt**에 입력해주면 이제 http://localhost 나 http://localhost:80를 통해 접속할 수 있다.   
하지만, 전과는 다르게 오류가 뜰 것이다. 그 이유는 디버그 툴바 때문인데, [디버깅 툴바](https://hwangtolee.github.io/python/django/2019/07/12/Python-Django-Debug-Toolbar.html)를 다룬 글에서 3번째 항목, **urls.py**를 수정하지 않았다면 오류가 뜰 것이다.

**<u>projname/urls.py</u>** 

```python
from django.contrib import admin
from django.urls import path
from django.conf.urls import url
from django.conf import settings
from survey import views
from django.urls import include

urlpatterns = [
    path('admin/', admin.site.urls),
]

if settings.DEBUG:
    import debug_toolbar
    urlpatterns += [
        url(r"^__debug__/", include(debug_toolbar.urls))
        ]
```

우선 **urls.py**를 이렇게 바꿔주자. 그 후, 다시 웹 페이지에 접속하면 우측에 디버깅툴바가 뜬 것을 볼 수 있다. **Page not found**가 뜨는 이유는 아직 **admin** 페이지와 **debug** 페이지말고는 만들지 않았기 때문이다. 

주소 뒤에 **/admin**을 붙여서 관리자페이지에 접속해서 로그인해준다.  
관리자 페이지에서 **Survey**에 설문조사를 하나 만들어서 저장해보자.

**SQLite** **Expert**를 통해 **db.sqlite3** 파일을 확인해보면, **survey_survey** 테이블에 내용이 추가된 것을 알 수 있다.  

### 6. Url과 페이지 작성

**<u>projname/urls.py</u>**  

```python
from django.contrib import admin
from django.urls import path
from django.conf.urls import url
from django.conf import settings
from survey import views
from django.urls import include

urlpatterns = [
    path('admin/', admin.site.urls),
    url(r"^$", views.main),
    url(r"^save_survey$", views.save_survey),
    url(r"^show_result$", views.show_result),
]

if settings.DEBUG:
    import debug_toolbar
    urlpatterns += [
        url(r"^__debug__/", include(debug_toolbar.urls))
        ]

```

**<u>survey/views.py</u>**  

```python
from django.shortcuts import render
from survey.models import Survey, Answer
from django.shortcuts import render_to_response, redirect
from django.views.decorators.csrf import csrf_exempt

def main(request):
    survey=Survey.objects.filter(status="y").order_by("-survey_idx")[0]
    
    return render_to_response("main.html", {"survey":survey})

@csrf_exempt
def save_survey(request):
    print(request.POST['survey_idx'])
    print(request.POST['num'])
    dto = Answer(survey_idx=request.POST['survey_idx'], num=request.POST['num'])
    dto.save()
    return render_to_response("success.html", {"dto":dto})

def show_result(request):
    idx = request.GET['survey_idx']
    
    ans = Survey.objects.get(survey_idx=idx)
    
    answer = [ans.ans1, ans.ans2, ans.ans3, ans.ans4]
    
    surveyList = Survey.objects.raw("""
    select
        survey_idx,num,count(num) sum_num,
        round((select count(*) from survey_answer
            where survey_idx=a.survey_idx
              and num=a.num)*100.0 /
            (select count(*) from survey_answer
             where survey_idx=a.survey_idx), 1) rate
    from survey_answer a
    where survey_idx=%s
    group by survey_idx,num
    order by num
    """, idx)
    surveyList=zip(surveyList, answer)
    return render_to_response("result.html",{"surveyList":surveyList})
```

**main(request)**함수는 **survey**들 중 **status**가 **"y"**인 것들만 내림차순으로 정렬한 후 제일 첫번째 것을 가지고 **main.html**에 자료를 보내서 렌더시켜준다.  
**save_survey(request)**에서 **print**함수는 **Anaconda Prompt**에서 값을 확인할 수 있게 도와준다.

**show_result(request)**함수에서 **Survey.objects.raw** 는 **SQL** 코드를 사용하고 싶을 때 사용하는 함수로, **raw** 뒤에 나오는 """이 것들"""을 **SQL** 코드로 입력할 수 있다.

**zip(surveyList, answer)**는 **surveyList**와 **answer**를 합쳐준다. (압축 파일 같은 역할)

**<u>survey/templates/main.html</u>**  

```html
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
<script>
function show_result(){
	location.href="show_result?survey_idx={{survey.survey_idx}}";
}
</script>
</head>
<body>
<h2>온라인 설문조사</h2>
<form method="post" action="save_survey">
{% raw %}{% csrf_token %}{% endraw %}
{{survey.question}}<br>
<input type="radio" name="num" value="1">{{survey.ans1}}<br>
<input type="radio" name="num" value="2">{{survey.ans2}}<br>
<input type="radio" name="num" value="3">{{survey.ans3}}<br>
<input type="radio" name="num" value="4">{{survey.ans4}}<br><br>
<input type="hidden" name="survey_idx" value="{{survey.survey_idx}}">
<input type="submit" value="투표">
<input type="button" value="결과 확인" onclick="show_result()">
</form>
</body>
</html>
```

**<u>survey/templates/success.html</u>**

```html
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<h2>온라인 설문조사</h2>
완료되었습니다.

<a href="/">Home</a>
<a href="show_result?survey_idx={{dto.survey_idx}}">설문결과</a>
</body>
</html>
```

**<u>survey/templates/result.html</u>**

```html
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<h2>설문조사 결과</h2>
<table border="1">
	<tr align="center">
		<th>문항</th>
		<th>응답수</th>
		<th>응답비율</th>
	</tr>
	{% for row,ans in surveyList %}
	<tr align="center">
		<td>{{ans}}</td>
		<td>{{row.sum_num}}</td>
		<td>{{row.rate}}</td>
	</tr>
	{% endfor %}
</table>
</body>
</html>
```







{% if page.comments %}
<div id="disqus_thread"></div>
<script>
/**
*  RECOMMENDED CONFIGURATION VARIABLES: EDIT AND UNCOMMENT THE SECTION BELOW TO INSERT DYNAMIC VALUES FROM YOUR PLATFORM OR CMS.
*  LEARN WHY DEFINING THESE VARIABLES IS IMPORTANT: https://disqus.com/admin/universalcode/#configuration-variables*/
/*
var disqus_config = function () {
this.page.url = PAGE_URL;  // Replace PAGE_URL with your page's canonical URL variable
this.page.identifier = PAGE_IDENTIFIER; // Replace PAGE_IDENTIFIER with your page's unique identifier variable
};
*/
(function() { // DON'T EDIT BELOW THIS LINE
var d = document, s = d.createElement('script');
s.src = 'https://hwnagto.disqus.com/embed.js';
s.setAttribute('data-timestamp', +new Date());
(d.head || d.body).appendChild(s);
})();
</script>
<noscript>Please enable JavaScript to view the <a href="https://disqus.com/?ref_noscript">comments powered by Disqus.</a></noscript>
{% endif %}