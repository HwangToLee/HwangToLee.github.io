---
title: Django 01. 파이썬 Django 사전준비
date: '2019-07-10 10:10:00'
categories: Python/Django
comments: true
---

이번 포스트부터는 파이썬 웹 프로그래밍 오픈소스 프레임워크인 Django에 대해서 다루어보려고 한다. 이 포스트에서는 앞으로 파이썬 Django를 할 때 필요한 기초적인 프로그램들을 소개하려고 한다. (이 포스트는 [YOUTUBE : LIFE SOFT](https://www.youtube.com/channel/UCqRTjWqD-ZWHj0ZoPSKVWBw) 님이 게시하시는 글을 통해 공부하고 작성한 것이다.)

## 1. Python, Django
당연하지만 Django는 파이썬 웹 프로그래밍 오픈소스 프레임워크이기 때문에 파이썬이 컴퓨터에 설치되어 있어야한다. 설치는 [파이썬 다운로드](https://www.python.org/downloads/)에서 간단하게 할 수 있다. 또한, Django를 이용하기 때문에 당연하게도 Django 또한 다운로드 받아야한다. Django 또한 오픈소스 프레임워크이기 때문에 [이 곳](https://www.djangoproject.com/)에서 손쉽게 다운로드 받을 수 있다.

## 2. Eclipse (Plugin : Pydev)

내가 공부한 곳에서는 Eclipse의 Pydev 플러그인을 사용한 환경에서 개발했기 때문에, 나도 이 환경을 사용했다. (필수 아님). 추가로, Eclipse에서 HTML을 Edit할 수 있는 플러그인도 마켓 플레이스에서 받아놓으면 좋다. Eclipse는 [이클립스 홈페이지](https://www.eclipse.org/)에서 무료로 받을 수 있다. Pydev는 Eclipse의 마켓 플레이스에서도 받을 수 있으며, 그 후, Eclipse 우측 상단에서 perspective를 Pydev로 바꿔주어야 한다. 또한, windows->preferences->PyDev->Interpreters->Python Interpreters에서 자신이 설치한 Python의 Python.exe파일을 연결시켜 주어야한다.

## 3. Anaconda Prompt
그냥 CMD창에서 하면 Python 3.7이상 버전부터는 오류가 있다는 듯 하다. 그리고 호환성도 그렇게 좋지 않기 때문에 [아나콘다 홈페이지](https://www.anaconda.com/distribution/)에서 무료로 받을 수 있는 Anaconda Prompt를 사용하는 것이 좋다.

## 4. SQLite Expert
SQLite Expert를 통해 데이터베이스를 보고 관리할 수 있다. SQLite Expert 또한 [SQLite Expert](http://www.sqliteexpert.com/)에서 Personal 버전은 무료로 받을 수 있다.

이 정도면 기본적인 준비는 끝났다고 생각한다. 다음 포스트에서는 Django에서 Bookmark앱을 만들어 볼 것이다.


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