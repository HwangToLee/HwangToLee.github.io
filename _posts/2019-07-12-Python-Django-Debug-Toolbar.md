---
title: Django A1. Django Debug Toolbar 설치
date: '2019-07-12 11:00:00'
categories: Python/Django
comments: true
---
이번 포스트는 번외편으로, Django의 디버깅 툴바를 설치해볼 것이다. 

(이 포스트는 [YOUTUBE : LIFE SOFT](https://www.youtube.com/channel/UCqRTjWqD-ZWHj0ZoPSKVWBw) 님이 게시하시는 글을 통해 공부하고 작성한 것이다.)

### 1. 패키지 설치

Anaconda Prompt에서 아래 코드를 입력한다.   
<code>pip install django-debug-toolbar</code>  
그러면 django-debug-toolbar가 성공적으로 설치될 것이다.

### 2. settings.py 수정

Django 디버깅 툴바의 설치를 완료했으니, 이제 사용하는 방법에 대해 알아보자.  
앞으로 만들 프로젝트들의  **projname/projname/settings.py**를 수정해야한다.   
우선 아래 코드를 추가해준다.

```python
INTERNAL_IPS = ('127.0.0.1',)
```
또한, **INSTALLED_APPS**라는 항목 안에 **‘debug_toolbar’,**를 추가해주고, **MIDDLEWARE**라는 항목 안에 **'debug_toolbar.middleware.DebugToolbarMiddleware'**,를 추가해준다.

### 3. urls.py 수정

**<u>projname/urls.py</u>**  

```python
from django.conf import settings

if settings.DEBUG:
    import debug_toolbar
    urlpatterns += [
        url(r'^__debug__/', include(debug_toolbar.urls)),
    ]
```

마지막으로, 앞으로 만들 프로젝트들의 url에도 이렇게 코드를 추가해주면 디버깅 툴바를 사용할 수 있다. 





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