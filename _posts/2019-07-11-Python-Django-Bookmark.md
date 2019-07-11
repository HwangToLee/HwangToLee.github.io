---
title: 파이썬 Django Bookmark 앱 만들기
date: '2019-07-11 18:00:00'
categories: Python/Django
---
[앞선 포스트](https://hwangtolee.github.io/python,/django/2019/07/10/Python-Django-Preperations.html)에서 파이썬 Django를 이클립스에서 사용하기 위한 최소한의 세팅을 끝냈다. 이번 포스트에서는 북마크 앱을 만들어 볼 것이다.

(이 포스트는 [YOUTUBE : LIFE SOFT](https://www.youtube.com/channel/UCqRTjWqD-ZWHj0ZoPSKVWBw) 님이 게시하시는 글을 통해 공부하고 작성한 것이다.)

### 1. 새로운 프로젝트 만들기
File -> New -> Other (Ctrl + N)을 누르면 다양한 옵션들이 뜰 것이다.  
Django를 검색하면 나오는 PyDev의 Django를 선택하고, 이름을 정해서 만들어주자.  
그러면 프로젝트가 생성되고, 프로젝트 명과 동일한 디렉토리와 manage.py가 생겼을 것이다. 

### 2. 기본 테이블 생성 및 관리자 계정 (super user) 생성
Anaconda Prompt에서 cd (change directory)명령어를 통해 현재 만든 프로젝트의 디렉토리로 들어간다. 예를 들어, D:\Projects에 testproj라는 프로젝트를 만들었다면  
<code> cd D:\Projects\testproj </code>  
이 코드를 입력하면 된다.  
**이제부터 하는 모든 명령어는 이렇게 해당 프로젝트의 디렉토리로 들어와서 해야한다.**

기본적인 명령어는 모두 django에서 만들어준 manage.py를 이용해서 한다.  
<code> python manage.py migrate </code>  
이 코드를 입력하면 db.sqlite3라는 기본 테이블이 해당 디렉토리에 생성된다. 앞선 포스트에서 설치했던 sqlite expert를 통해서 이 파일을 열어보고 내용을 확인할 수 있다. 

또한, Django에서는 관리자 페이지 (admin page)를 기본적으로 제공해준다. 그래서 우리는 관리자 계정만 만들어주면 된다.  
<code> python manage.py createsuperuser  </code>  
이 코드를 입력하면 관리자 계정을 만드는 프로세스에 들어간다. 계정, 이메일, 비밀번호를 순서대로 입력하게 되는데, 이메일은 필수 사항이 아니라서 그냥 엔터만 쳐도 된다. 비밀번호는 영문과 숫자를 포함해야하며 8자이상이여야한다는 조건이 있다.

### 3. 북마크 앱 만들기
이제 북마크 앱을 만들어보려고 한다. Anaconda prompt에서 해당 프로젝트의 디렉토리에 다음 코드를 입력한다.  
<code> python manage.py startapp bookmark </code>  
그러면 해당 디렉토리에 북마크 앱에 관련된 디렉토리가 추가된다. 이 작업 후에 프로젝트 이름 / 프로젝트 이름 디렉토리에 있는 settings.py를 조금 수정해야한다. INSTALLED_APPS라는 항목 안에 ‘bookmark’,를 추가해주고, LANGUAGE_CODE을 ‘ko-kr’, TIME_ZONE을 ‘Asia/Seoul’로 수정해준다.

### 4. 모델 클래스 정의와 Admin 사이트 설정
새로운 테이블을 만들었으면, models.py에서 해당 테이블에 대한 모델 클래스를 정의해야 하고, models.py에 등록한 그 테이블을 관리자 페이지에서 보이도록 만들어야 한다.

**bookmark/models.py**  
```python
from django.db import modelsclass

Bookmark(models.Model):
    title=models.CharField(max_length=100,blank=True,null=True)
    url=models.URLField("url",unique=True)
    def __str__(self):
        return self.title
```

django에서 지원하는 models의 Model이라는 클래스를 상속하는 자식 클래스 Bookmark를 만들고, title과 url 필드를 만들어서 초기화시켜준다.  또한, 출력시 title이 나오도록 설정해놓았다. 

**bookmark/admin.py**  

```python
from django.contrib import admin
from bookmark.models import Bookmark
class BookmarkAdmin(admin.ModelAdmin):
    list_display = ('title','url')
admin.site.register(Bookmark,BookmarkAdmin)
```

BookmarkAdmin 클래스는 방금 만든 Bookmark 클래스가 관리자 페이지에서 나타나는 모습을 정의하는 클래스이다. 이 클래스 또한 admin.ModelAdmin 클래스를 상속시켜준다.   
admin.site.register(Bookmark, BookmarkAdmin) 함수를 통해 Bookmark 클래스와 BookmarkAdmin 클래스를 관리자 페이지에 등록했다.  

이제 이렇게 만든 모델 클래스들을 데이터베이스에 반영해야한다.   
<pre><code>python manage.py makemigrations
python manage.py migrate</code>
</pre>

### 5. 웹서버 구동과 관리자 페이지

<code>python manage.py runserver localhost:80</code>

이 코드를 사용하고 http://localhost 나 http://localhost:80에 접속하면 **성공적으로 설치되었습니다! 축하합니다!** 라는 메시지가 중간에 있는 웹 사이트가 뜰 것이다. http에서는 80이 기본 포트이기 때문에 80을 붙이든 붙이지않든 같은 결과가 나온다. 우리는 관리자 페이지를 제외하고는 만들지 않았기 때문에 저 웹 사이트가 뜨는 것이다. 관리자 페이지는 주소 뒤에 **/admin**을 붙여서 접속할 수 있다.    

이전에 작성했던 관리자 계정을 통해 로그인할 수 있다.  
로그인 하면 관리자 페이지가 나오고, Bookmark에 있는 bookmarks를 추가할 수 있다. 아까 우리가 만들었던 틀 대로 Title과 Url을 넣을 수 있고, Title: 구글 Url: https://www.google.com/ 와 같은 방식으로 페이지를 몇개 추가해보자.   

SQLite Expert를 통해 db.sqlite3 파일을 확인해보면, bookmark_bookmark 테이블에 내용이 추가된 것을 알 수 있다.  

### 6. Url과 페이지 작성
**projname/urls.py**  
```python
from django.contrib import admin
from django.conf.urls import url
from bookmark import views

urlpatterns = [
    url(r"^admin/", admin.site.urls),
    url(r"^$", views.home),
    url(r"^detail$", views.detail)
]
```

우선 위와 같이 url 패턴을 정의해준다.  
r은 정규 표현식을 뜻하고, ^는 시작을, $은 끝을 뜻한다. 즉, r"^$"는 아무것도 없다는 뜻이다.  
views.home과 views.detail은 우리가 views.py에서 지금 만들 것이다.  
**bookmark/views.py**  

```python
from bookmark.models import Bookmark
from django.shortcuts import render_to_response

def home(request):
    urlList=Bookmark.objects.order_by("title")
    urlCount=Bookmark.objects.all().count()
    return render_to_response(
        "list.html", {"urlList":urlList, "urlCount":urlCount})

def detail(request):
    addr=request.GET["url"]
    dto=Bookmark.objects.get(url=addr)
    return render_to_response("detail.html",{"dto":dto}) 
```

**bookmark/templates/list.html**

```html
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<div id="container">
<h1>Bookmark List</h1>
<ul>
	{% for row in urlList %}
		<li>
			<a href="detail?url={{row.url}}">{{row.title}}</a>
		</li>
	{% endfor %}
</ul>
</div>
</body>
</html>
```

**bookmark/templates/detail.html**
```html
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<div id="container">
<h1>{{dto.title}}</h1>
<ul>
<li>URL:<a href="{{dto.url}}">{{dto.url}}</a></li>
</ul>
</div>
</body>
</html>
```
views.py에 있는 함수 home과 detail은 각각 템플릿 list.html, detail.html에 주어진 데이터를 대입하면서 불러온다. 중괄호 2개는 해당 데이터를 대입한다는 뜻이다.

즉, 작동방식은 이와 같다. url patterns를 보고 현재	 url에 해당하는 함수를 호출한다. 해당하는 함수는 호출되면 템플릿에 데이터를 집어넣어서 유저에게 보여준다. 