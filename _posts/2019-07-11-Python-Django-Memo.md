---
title: 파이썬 Django Bookmark 앱 만들기
date: '2019-07-11 11:00:00'
categories: Python/Django
---
이번 포스트에서는 한줄 메모장을 만들어볼 것이다.

(이 포스트는 [YOUTUBE : LIFE SOFT](https://www.youtube.com/channel/UCqRTjWqD-ZWHj0ZoPSKVWBw) 님이 게시하시는 글을 통해 공부하고 작성한 것이다.)

### 1. 새로운 프로젝트 만들기 및 기본 테이블, 관리자 계정 생성

여기까지는 [저번포스트](https://hwangtolee.github.io/python/django/2019/07/11/Python-Django-Bookmark.html)의 1번, 2번 과정과 동일하다. 

### 2. 메모장 앱 만들기 및 세팅

Anaconda prompt에서 해당 프로젝트의 디렉토리에 다음 코드를 입력한다.  
`python manage.py startapp memo `
그러면 해당 디렉토리에 메모장 앱에 관련된 디렉토리가 추가된다.  
이 작업 후에  projname/projname/settings.py를 조금 수정해야한다.   
맨 위에 아래 코드를 추가해줘서 날짜 및 시간을 한국식으로 나오게 조정해준다.

```python
from django.conf.locale.ko import formats as ko_formats
ko_formats.DATETIME_FORMAT = 'Y-m-d G:i:s'
```
또한, INSTALLED_APPS라는 항목 안에 ‘memo’,를 추가해주고, LANGUAGE_CODE을 ‘ko-kr’, TIME_ZONE을 ‘Asia/Seoul’로 수정해준다.

### 3. 모델 클래스 정의와 Admin 사이트 설정

새로운 테이블을 만들었으면, models.py에서 해당 테이블에 대한 모델 클래스를 정의해야 하고, models.py에 등록한 그 테이블을 관리자 페이지에서 보이도록 만들어야 한다.

**bookmark/models.py**  

```python
from django.db import models
from datetime import datetime

class Memo(models.Model):
    idx = models.AutoField(primary_key=True)
    writer=models.TextField(null=False)
    memo=models.TextField(null=False)
    
    post_date=models.DateTimeField(default=datetime.now,blank=True)
```

django에서 지원하는 models의 Model이라는 클래스를 상속하는 자식 클래스 Memo를 만들고, 

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

### 4. 웹서버 구동과 관리자 페이지

