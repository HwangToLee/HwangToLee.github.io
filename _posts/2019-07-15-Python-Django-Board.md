---
title: Django 07. Django 게시판 만들기
date: '2019-07-15 21:30:00'
categories: Python/Django
comments: true
---
이번 포스트에서는 Django로 회원가입과 로그인을 구현해볼 것이다.

(이 포스트는 [YOUTUBE : LIFE SOFT](https://www.youtube.com/channel/UCqRTjWqD-ZWHj0ZoPSKVWBw) 님이 게시하시는 글을 통해 공부하고 작성한 것이다.)

### 1. 새로운 프로젝트 만들기

**File -> New -> Other (Ctrl + N)**을 누르면 다양한 옵션들이 뜰 것이다.  
**Django**를 검색하면 나오는 **PyDev**의 **Django**를 선택하고, 이름을 정해서 만들어주자.   

### 2. 기본 테이블 생성 및 관리자 계정 (super user) 생성

<code> python manage.py migrate </code>  
**Anaconda Prompt**에서 해당 프로젝트의 디렉토리로 간 뒤 이 코드를 입력해서 **db.sqlite3**라는 기본 테이블을 해당 디렉토리에 생성하자.  
<code> python manage.py createsuperuser  </code>  
또한, 이 코드를 입력해서 관리자 계정을 만들어주자.

### 3. 게시판 앱 만들기 및 세팅

**Anaconda prompt**에서 해당 프로젝트의 디렉토리에 다음 코드를 입력한다.  

<pre><code>python manage.py startapp board </code></pre>
그러면 해당 디렉토리에 메모장 앱에 관련된 디렉토리가 추가된다.  
이 작업 후에  **projname/settings.py**를 조금 수정해야한다.   
또한, **INSTALLED_APPS**라는 항목 안에 **‘board’,**를 추가해주고, **LANGUAGE_CODE**을 **‘ko-kr’**, **TIME_ZONE**을 **‘Asia/Seoul’**로 수정해준다.

### 4. 모델 클래스 정의와 Admin 사이트 설정

**<u>board/models.py</u>**  

```python
from django.db import models
from datetime import datetime
from builtins import False

class Board(models.Model):
    idx = models.AutoField(primary_key=True)
    writer = models.CharField(null=False, max_length=50)
    title = models.CharField(null=False, max_length=120)
    hit = models.IntegerField(default=0)
    content = models.TextField(null=False)
    post_date = models.DateTimeField(default=datetime.now, blank=True)
    filename = models.CharField(null=True, blank=True, default="", max_length=500)
    filesize = models.IntegerField(default=0)
    down = models.IntegerField(default=0)
    
    def hit_up(self):
        self.hit += 1
    def down_up(self):
        self.down += 1
        
        
class Comment(models.Model):
    idx = models.AutoField(primary_key=True)
    board_idx = models.IntegerField(null=False)
    writer = models.CharField(null=False, max_length=50)
    content = models.TextField(null=False)
    post_date = models.DateTimeField(default=datetime.now, blank=True)
```

**Model**을 상속받는 클래스 **Board**를 만들었다. 위에서부터 차례대로 글번호, 글쓴이, 제목, 조회수, 본문, 작성날짜, 첨부파일 이름, 첨부파일 크기, 다운로드 횟수이다. 

**Model**을 상속받는 또 다른 클래스 **Comment**도 만들었다. 댓글에 대한 클래스이고, 위에서부터 차례대로 댓글번호, 글번호 (댓글이 달리는 게시글의 번호), 작성자, 내용, 작성날짜이다.

**<u>board/admin.py</u>**  

```python
from django.contrib import admin
from board.models import Board, Comment

class BoardAdmin(admin.ModelAdmin):
    list_display = ("writer", "title", "content")

class CommentAdmin(admin.ModelAdmin):
    list_display = ("writer", "content")

admin.site.register(Board, BoardAdmin)
admin.site.register(Comment, CommentAdmin)
```

**BoardAdmin, CommentAdmin** 클래스를 만들어서 **Board, Comment**라는 클래스가 관리자페이지에서 어떻게 보일지 정의해준다. 

**admin.site.register()** 함수를 통해 **Board**와 **BoardAdmin**, **Comment**와 **CommentAdmin** 클래스를 관리자 페이지에 등록해준다.  

이제 이렇게 만든 모델 클래스들을 데이터베이스에 반영해준다.     

<pre><code>python manage.py makemigrations
python manage.py migrate</code>
</pre>

### 5.  웹서버 구동과 관리자 페이지

<code>python manage.py runserver localhost:80</code>

이 코드를 **Anaconda** **Prompt**에 입력해주면 이제 http://localhost 나 http://localhost:80를 통해 접속할 수 있다.  
주소 뒤에 **/admin**을 붙여서 관리자페이지에 접속해서 로그인해준다.  
관리자 페이지에서 **Boards**에 글을 몇 개 추가해보자.

**SQLite** **Expert**를 통해 **db.sqlite3** 파일을 확인해보면, **board_board** 테이블에 내용이 추가된 것을 알 수 있다.  

### 6. Url과 페이지 작성

**<u>projname/urls.py</u>**  

```python
from django.contrib import admin
from django.urls import path
from board import views

urlpatterns = [
    path('admin/', admin.site.urls),
    
    path('', views.list),
    path('write', views.write),
    path('insert', views.insert),
    path('download', views.download),
    path('detail', views.detail),
    path('update', views.update),
    path('delete', views.delete),
    path('reply_insert', views.reply_insert),
]

```

**path** 함수의 3번째 인수 **name**은 별칭으로, 후에 템플릿에서 **<code>{% raw %}{% url 'name' %}{% endraw %}</code>**을 사용함으로써 해당 주소를 불러올 수 있다.

**<u>board/views.py</u>**  

```python
from django.shortcuts import render
from board.models import Board, Comment
from django.shortcuts import redirect, render_to_response
import os
import math
from django.views.decorators.csrf import csrf_exempt
from django.utils.http import urlquote
from django.http import HttpResponse, HttpResponseRedirect
from django.db.models import Q
UPLOAD_DIR = "d:/upload/"
@csrf_exempt
def list(request):
    #검색 기능
    try:
        search_option=request.POST["search_option"]
    except:
        search_option="writer"
    try:
        search=request.POST["search"]
    except:
        search=""
    #검색된 게시글들의 개수 세기
    if search_option=="all":
        boardCount=Board.objects.filter(Q(writer__contains=search)
        |Q(title__contains=search)|Q(content__contains=search)).count()
    elif search_option=="writer":
        boardCount=Board.objects.filter(Q(writer__contains=search)).count()
    elif search_option=="title":
        boardCount=Board.objects.filter(Q(title__contains=search)).count()
    elif search_option=="content":
        boardCount=Board.objects.filter(Q(content__contains=search)).count()
    #페이지 나누기
    try:
        start=int(request.GET["start"])
    except:
        start=0
    page_size = 10
    page_list_size = 10
    end = start + page_size
    total_page = math.ceil(boardCount / page_size)
    current_page=math.ceil( (start+1) / page_size )
    start_page = math.floor( (current_page - 1) / page_list_size)\
        * page_list_size + 1
    end_page = start_page + page_list_size - 1
    
    if total_page < end_page:
        end_page = total_page
    if start_page >= page_list_size:
        prev_list = (start_page - 2) * page_size
    else:
        prev_list = 0
    if total_page > end_page:
        next_list = end_page * page_size
    else:
        next_list = 0
    #화면에 표시할 게시글들 리스트
    if search_option=="all":
        boardList=Board.objects.filter(Q(writer__contains=search)
        |Q(title__contains=search)|Q(content__contains=search)).order_by("-idx")[start:end]
    elif search_option=="writer":
        boardList=Board.objects.filter(Q(writer__contains=search)).order_by("-idx")[start:end]
    elif search_option=="title":
        boardList=Board.objects.filter(Q(title__contains=search)).order_by("-idx")[start:end]
    elif search_option=="content":
        boardList=Board.objects.filter(Q(content__contains=search)).order_by("-idx")[start:end]
    #하단의 네비게이션 바
    links = []
    for i in range(start_page, end_page+1):
        page = (i-1) * page_size
        links.append("<a href='?start="+str(page)+"'>"+str(i)+"</a>")
    
    return render_to_response("list.html", 
                    {"boardList":boardList, "boardCount":boardCount,
                     "search_option":search_option, "search":search,
                     "range":range(start_page-1, end_page),
                     "start_page":start_page, "end_page":end_page,
                     "page_list_size":page_list_size, "total_page":total_page,
                     "prev_list":prev_list, "next_list":next_list,
                     "links":links})

def write(request):
    return render_to_response("write.html")

@csrf_exempt
def insert(request):
    fname=""
    fsize=0
    if "file" in request.FILES:
        file = request.FILES["file"]
        print(file)
        fname = file._name
        
        print(UPLOAD_DIR+fname)
        with open("%s%s" % (UPLOAD_DIR, fname), "wb") as fp:
            for chunk in file.chunks():
                fp.write(chunk)
            
        fsize = os.path.getsize(UPLOAD_DIR+fname)
        
    dto = Board(writer = request.POST["writer"],
                title = request.POST["title"],
                content = request.POST["content"],
                filename = fname, filesize = fsize)
    dto.save()
    return redirect("/")

def download(request):
    id = request.GET["idx"]
    dto = Board.objects.get(idx=id)
    path = UPLOAD_DIR+dto.filename
    filename = os.path.basename(path)
    filename=urlquote(filename)
    with open(path, "rb") as file:
        response = HttpResponse(file.read(),
                content_type="application/octet-stream")
        response["Content-Disposition"]=\
        "attachment;filename*=UTF-8''{0}".format(filename)
        dto.down_up()
        dto.save()
    return response
    
def detail(request):
    id = request.GET["idx"]
    dto = Board.objects.get(idx=id)
    dto.hit_up()
    dto.save()
    filesize="%.2f" % (dto.filesize / 1024)
    
    commentList=Comment.objects.filter(board_idx=id).order_by("-idx")
    
    return render_to_response("detail.html", 
        {"dto":dto, "filesize":filesize, "commentList":commentList})

@csrf_exempt
def update(request):
    id = request.POST["idx"]
    dto_src=Board.objects.get(idx=id)
    
    fname = dto_src.filename
    fsize = dto_src.filesize
    if "file" in request.FILES:
        file = request.FILES["file"]
        fname = file._name
        fp = open("%s%s" % (UPLOAD_DIR, fname), "wb")
        for chunk in file.chunks():
            fp.write(chunk)
        fp.close()
        fsize = os.path.getsize(UPLOAD_DIR+fname)
        
    dto_new = Board(idx=id, writer=request.POST["writer"],
        title=request.POST["title"], content=request.POST["content"],
        filename=fname, filesize=fsize)
    dto_new.save()
    return redirect("/")

@csrf_exempt
def delete(request):
    id = request.POST["idx"]
    Board.objects.get(idx=id).delete()
    return redirect("/")

@csrf_exempt
def reply_insert(request):
    id = request.POST["idx"]
    dto = Comment(board_idx=id, writer=request.POST["writer"],
                  content=request.POST["content"])
    dto.save()
    return HttpResponseRedirect("detail?idx="+id)
```

**List** 함수가 상당히 복잡해서 주석으로 설명을 조금 추가해놓았다.  
**List** 함수의 검색 기능은 [저번 포스트](https://hwangtolee.github.io/python/django/2019/07/15/Python-Django-Signup-Login.html)를 참고하면 된다. 저번 포스트에서 화면에 표시할 게시글들의 리스트를 만들 때 페이지를 분할해서 **[start:end]**의 범위에서만 지정한다는 점만 추가되었다.  
페이지를 나누는 기능은 기본적으로 **start**변수에서 한 페이지에 표시할 게시글의 수 **page_size**만큼 더해서 끝 번호 **end**를 구하고, 화면에 표시할 게시글들의 리스트를 만들었을 때 그 **start**부터 **end**사이의 인덱스인 글만 표시하는 것이다. 현재 5페이지에서 검색을 했을 때 5페이지에 그대로 있는 문제점이 있다. (검색한 분량이 1페이지만큼 인데도 5페이지에 있어서 결과가 안나온다.)  

**reply_insert**의 **HttpResponseRedirect** 함수는 **redirect**와는 다르게 "url"+id처럼 다른 변수를 추가할 수 있다. (**redirect**함수는 오로지 url만 줄 수 있다.)

**<u>board/templates/list.html</u>**  

```html
{% raw %}{% load staticfiles %}
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<h2>게시판</h2>
<form method="post">
{% csrf_token %}
<select name="search_option">
{% if search_option == "writer" %}
	<option value="writer" selected>이름</option>
	<option value="title">제목</option>
	<option value="content">내용</option>
	<option value="all">이름+제목+내용</option>
{% elif search_option == "title" %}
	<option value="writer">이름</option>
	<option value="title" selected>제목</option>
	<option value="content">내용</option>
	<option value="all">이름+제목+내용</option>
{% elif search_option == "content" %}
	<option value="writer">이름</option>
	<option value="title">제목</option>
	<option value="content" selected>내용</option>
	<option value="all">이름+제목+내용</option>
{% elif search_option == "all" %}
	<option value="writer">이름</option>
	<option value="title">제목</option>
	<option value="content">내용</option>
	<option value="all" selected>이름+제목+내용</option>
{% endif %}
</select>
<input type="text" name="search" value="{{search}}">
<input type="submit" value="검색">
</form>
게시물수 : {{boardCount}}
<a href="write">글쓰기</a>
<table border="1">
	<tr>
		<th>번호</th>
		<th>이름</th>
		<th>제목</th>
		<th>날짜</th>
		<th>조회수</th>
		<th>첨부파일</th>
		<th>다운로드</th>
	</tr>
	{% for row in boardList %}
	<tr>
		<td>{{row.idx}}</td>
		<td>{{row.writer}}</td>
		<td><a href="detail?idx={{row.idx}}">{{row.title}}</a></td>
		<td>{{row.post_date}}</td>
		<td>{{row.hit}}</td>
		<td>
			{% if row.filesize > 0 %}
			<a href="download?idx={{row.idx}}">
				<img src="{% static "images/file.gif" %}"</a>
			{% endif %}
		</td>
		<td>{{row.down}}</td>
	</tr>
	{% endfor %}
	<tr>
		<td colspan="7" align="center">
			{% if start_page >= page_list_size %}
				<a href="?start={{prev_list}}">[이전]</a>
			{% endif %}
			{% autoescape off %}
				{% for link in links %}
					{{link}}
				{% endfor %}
			{% endautoescape %}
			{% if total_page > end_page %}
				<a href="?start={{next_list}}">[다음]</a>	
			{% endif %}
		</td>
	</tr>
</table>
</body>
</html>{% endraw %}
```

기본 화면에 대한 템플릿이다. 맨 위는 검색기능에 대한 것이고, 그 다음은 게시글들을 보여주는 부분이다.  
맨 밑 네비게이션 바에 대한 설명:  
**page_list_size**(하단의 네이게이션 바에 나오는 페이지의 수)가 10이고, 게시글이 101개라고 치자. 처음에는 **start**가 0이고, 그에 따라 **start_page**는 1, **end_page**는 10이 된다. **total_page**는 11이 된다. 그러면 처음에는 맨밑에 1, 2, ... , 10, [다음]이 나오고, [다음]을 누르면 [이전], 11 이 나온다.

**<u>board/templates/write.html</u>**

```html
{% raw %}<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<h2>글쓰기</h2>
<form id="form1" name="form1" method="post"
 action="insert" enctype="multipart/form-data">
{% csrf_token %}
<div>이름<input type="text" name="writer" size="80"
 placeholder="이름을 입력하세요"></div>
<div>제목<input type="text" name="title" size="80"
 placeholder="제목을 입력하세요"></div>
<div style="width:800px;">내용<textarea rows="3" cols="80"
 name="content" placeholder="내용을 입력하세요"></textarea>
<div style="width:800px;">첨부파일<input type="file" name="file"></div>
<div style="width:800px;text-algin:center;">
	<button type="submit" id="btnSave">확인</button>
</div>
</form>
</body>
</html>{% endraw %}
```

**post** 메소드로 게시글에 대한 폼을 받아서 전달한다. 확인 버튼을 누르면 **'/insert'**웹 페이지로 이동한다.  
**enctype**은 첨부 파일 기능을 구현하기 위해 필요한 태그로, 여러 형식 중 하나다.

**<u>board/templates/detail.html</u>**

```html
{% raw %}<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
<script>
function home(){
	location.href="/";
}
function update(){
	document.form1.action="update";
	document.form1.submit();
}
function del(){
	if(confirm("삭제하시겠습니까?")){
	document.form1.action="delete";
	document.form1.submit();
	}
}
</script>
</head>
<body>
<h2>게시물 편집</h2>
<form method="post" name="form1" enctype="multipart/form-data">
<table border="1" width="700px">
	<tr>
		<td>이름</td>
		<td><input type="text" name="writer" value="{{dto.writer}}"></td>
	</tr>
	<tr>
		<td>제목</td>
		<td><input type="text" name="title" value="{{dto.title}}"></td>
	</tr>
	<tr>
		<td>날짜</td>
		<td>{{dto.post_date}}</td>
	</tr>
	<tr>
		<td>조회수</td>
		<td>{{dto.hit}}</td>
	</tr>
	<tr>
		<td>내용</td>
		<td><textarea rows="5" cols="60" name="content">{{dto.content}}</textarea></td>
	</tr>
	<tr>
		<td>첨부파일</td>
		<td>
			{% if dto.filesize > 0 %}
			<a href="download?idx={{dto.idx}}">{{dto.filename}}</a>
			( {{filesize}}KB)
			{% endif %}
			<input type="file" name="file"></td>
		</tr>
		<tr>
			<td colspan="2" align="center">
				<input type="hidden" name="idx" value="{{dto.idx}}">
				<input type="button" value="목록" onclick="home()">
				<input type="button" value="수정" onclick="update()">
				<input type="button" value="삭제" onclick="del()">
			</td>
		</tr>
</table>
</form>

<form method="post" action="reply_insert">
{% csrf_token %}
<input type="text" name="writer" placeholder="이름"><br>
<textarea rows="5" cols="80" name="content"
	placeholder="댓글을 작성하세요."></textarea><br>
<input type="hidden" name="idx" value="{{dto.idx}}">
<button>댓글쓰기</button>
</form>
<table border="1" width="700px">
	{% for row in commentList %}
	<tr>
		<td>{{row.writer}} ( {{row.post_date}} )<br>
			{{row.content}}
		</td>
	</tr>
	{% endfor %}
</table>
</body>
</html>{% endraw %}
```

게시글의 제목을 눌렀을 때 나오는 상세보기 화면이다.   
맨 밑에는 댓글 기능이 있다.





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