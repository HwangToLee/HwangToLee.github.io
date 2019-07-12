---
title: Python 03. 파이썬 Django Memo 앱 만들기
date: '2019-07-12 09:00:00'
categories: Python/Django
---
이번 포스트에서는 한줄 메모장을 만들어볼 것이다.  
-쓰다보니 전 포스트와 내용이 겹치는 것이 많아서 겹치는 부가 설명을 조금 배제했다.  
그에 대한 세부적인 설명을 원한다면 [저번 포스트](https://hwangtolee.github.io/python/django/2019/07/11/Python-Django-Bookmark.html)를 보는 것을 추천한다.

(이 포스트는 [YOUTUBE : LIFE SOFT](https://www.youtube.com/channel/UCqRTjWqD-ZWHj0ZoPSKVWBw) 님이 게시하시는 글을 통해 공부하고 작성한 것이다.)

### 1~2. 새로운 프로젝트 만들기 및 기본 테이블, 관리자 계정 생성

여기까지는 [저번 포스트](https://hwangtolee.github.io/python/django/2019/07/11/Python-Django-Bookmark.html)의 1번, 2번 과정과 동일하다. 

### 3. 메모장 앱 만들기 및 세팅

**Anaconda prompt**에서 해당 프로젝트의 디렉토리에 다음 코드를 입력한다.  

<pre><code>python manage.py startapp memo </code></pre>
그러면 해당 디렉토리에 메모장 앱에 관련된 디렉토리가 추가된다.  
이 작업 후에  **projname/projname/settings.py**를 조금 수정해야한다.   
맨 위에 아래 코드를 추가해줘서 날짜 및 시간을 한국식으로 나오게 조정해준다.

```python
from django.conf.locale.ko import formats as ko_formats
ko_formats.DATETIME_FORMAT = 'Y-m-d G:i:s'
```
또한, **INSTALLED_APPS**라는 항목 안에 **‘memo’,**를 추가해주고, **LANGUAGE_CODE**을 **‘ko-kr’**, **TIME_ZONE**을 **‘Asia/Seoul’**로 수정해준다.

### 4. 모델 클래스 정의와 Admin 사이트 설정

**<u>memo/models.py</u>**  

```python
from django.db import models
from datetime import datetime

class Memo(models.Model):
    idx = models.AutoField(primary_key=True)
    writer=models.TextField(null=False)
    memo=models.TextField(null=False)
    
    post_date=models.DateTimeField(default=datetime.now,blank=True)
```

**django**에서 지원하는 **models**의 **Model**이라는 클래스를 상속하는 자식 클래스 **Memo**를 만들어준다.  
**idx**는 **AutoField**로, 자동으로 증가해서 시리얼넘버를 주는 필드고 **primary key**로 설정해준다.  
나머지 2개는 **TextField**인데, **null=False**로 설정해서 무조건 내용이 있어야한다.  
**post_date**는 날짜와 시간과 관련된 필드인데, 디폴트 값을 현재 날짜와 시각으로 설정해주었다.   

**<u>memo/admin.py</u>**  

```python
from django.contrib import admin
from memo.models import Memo

class MemoAdmin(admin.ModelAdmin):
    list_display = ("writer","memo")
    
admin.site.register(Memo, MemoAdmin)
```

**MemoAdmin** 클래스를 만들어서 **Memo**라는 클래스가 관리자페이지에서 어떻게 보일지 정의해준다.   
**admin.site.register()** 함수를 통해 **Memo**와 **MemoAdmin** 클래스를 관리자 페이지에 등록해준다.  

이제 이렇게 만든 모델 클래스들을 데이터베이스에 반영해준다.   

<pre><code>python manage.py makemigrations
python manage.py migrate</code>
</pre>

### 5.  웹서버 구동과 관리자 페이지

### <code>python manage.py runserver localhost:80</code>

이 코드를 **anaconda** **prompt**에 입력해주면 이제 http://localhost 나 http://localhost:80를 통해 접속할 수 있다.  
주소 뒤에 **/admin**을 붙여서 관리자페이지에 접속해서 로그인해준다.  
관리자 페이지에서 **Writer**과 **Memo**에 텍스트를 입력하고, 날짜와 시각을 설정해서 저장할 수 있다. 몇가지를 저장해보자. 

**SQLite** **Expert**를 통해 **db.sqlite3** 파일을 확인해보면, **memo_memo** 테이블에 내용이 추가된 것을 알 수 있다.  

### 6. Url과 페이지 작성

**<u>projname/urls.py</u>**  

```python
from django.contrib import admin
from django.urls import path
from django.conf.urls import url
from memo import views

urlpatterns = [
    path('admin/', admin.site.urls),
    url(r'^$', views.home),
    url(r'^insert_memo$', views.insert_memo),
    url(r'^detail$', views.detail_memo),
    url(r'^update_memo$', views.update_memo),
    url(r'^delete_memo$', views.delete_memo),
]

```

**<u>memo/views.py</u>**  

```python
from django.shortcuts import redirect, render_to_response
from django.views.decorators.csrf import csrf_exempt
from memo.models import Memo

def home(request):
    memoList = Memo.objects.order_by("-idx")
    memoCount = Memo.objects.all().count()
    return render_to_response('list.html',{'memoList':memoList,'memoCount':memoCount})

@csrf_exempt
def insert_memo(request):
    memo = Memo(writer=request.POST['writer'], memo=request.POST['memo'])
    memo.save()
    return redirect("/")

def detail_memo(request):
    id=request.GET['idx']
    dto=Memo.objects.get(idx=id)
    return render_to_response('detail.html',{'dto':dto})

@csrf_exempt
def update_memo(request):
    id=request.POST['idx']
    memo=Memo(idx=id, writer=request.POST['writer'], memo=request.POST['memo'])
    memo.save()
    return redirect("/")

@csrf_exempt
def delete_memo(request):
    id=request.POST['idx']
    Memo.objects.get(idx=id).delete()
    return redirect("/")
```

**home(request)**함수에서 **memoList**에 **"-idx"**를 사용한 것은 내림차순으로 정렬하라는 뜻이다. ("-" 없으면 오름차순 정렬)  
**csrf_exempt**와 바로 밑에 나올 **csrf_token**은 **Cross Site Scripting Attack(크로스 사이트스크립팅 공격)**을 방지하기 위한 코드로, Django에서는 이런식으로 **input** 태그등을 사용할 때 필수적으로 넣어야한다고 한다.  
**memo.save** 함수는 **insert_memo**와 같이 **id**가 없을  때는 데이터 추가를, **update_memo**와 같이 id가 있을 때는 데이터 수정을 해준다.  
**return redirect("/")**는 함수가 끝난 뒤 기본 화면 ("http://localhost")로 돌아가게 해준다.

**<u>memo/templates/list.html</u>**  

```html
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
	<h2>한줄메모장</h2>
	메모 갯수 : {{memoCount}}
	<form method="post" action="insert_memo">
        {% raw %}
        {% csrf_token %}
        {% endraw %}
		이름 : <input name="writer">
		메모 : <input name="memo">
		<input type="submit" value="확인">
	</form>
	<table border="1">
		<tr>
			<th>번호</th>
			<th>이름</th>
			<th>메모</th>
			<th>날짜</th>
		</tr>
		{% for row in memoList %}
		<tr>
			<td>{{row.idx}}</td>
			<td>{{row.writer}}</td>
			<td><a href="detail?idx={{row.idx}}">{{row.memo}}</a></td>
			<td>{{row.post_date}}</td>
		</tr>
		{% endfor %}
	</table>
</body>
</html>
```

기본 화면에서는 for 문을 이용해서 DB에 있는 메모들을 번호, 이름, 메모(내용), 날짜 순으로 출력해준다. 메모 내용에 링크를 걸어서 http://localhost/detail?idx=(해당 인덱스 번호)로 갈 수 있게 해준다.  


**<u>memo/templates/detail.html</u>**

```html
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<script>
function update(){
	document.form1.action="update_memo"
	document.form1.submit();
}
function del(){
	if(confirm("삭제하시겠습니까?")){
	document.form1.action="delete_memo"
	document.form1.submit();		
	}
}
</script>
<title>Insert title here</title>
</head>
<body>
	<h2>메모 편집</h2>
	<form method="post" name="form1">
		<table border="1">
			<tr>
				<td>이름</td>
				<td><input name="writer" value="{{dto.writer}}"></td>
			</tr>
			<tr>
				<td>날짜</td>
				<td>{{dto.post_date}}</td>
			</tr>
			<tr>
				<td>메모</td>
				<td><input name="memo" value="{{dto.memo}}"></td>
			</tr>
			<tr>
				<td colspan="2" align="center">
					<input type="hidden" name="idx" value="{{dto.idx}}">
					<input type="button" value="수정" onclick="update()">
					<input type="button" value="삭제" onclick="del()">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>
```

**details** 화면에서는 수정과 삭제가 가능하다. **form**의 **input**을 사용해서 이름과 메모를 수정할 수 있고, 수정 버튼을 누르면 **update()**함수를 통해 DB가 업데이트되고, **views.py**에서 썼던 **redirect("/")**함수를 통해 기본 페이지로 돌아간다. 삭제 또한 삭제 후 **redirect("/")**를 통해 기본 페이지로 돌아간다.  
**idx**값 또한 수정이나 삭제할 때 넘겨줘야하기 때문에, 사용자에게는 필요없는 정보라서 **type="hidden"**으로 설정해줘서 데이터로만 전달한다.