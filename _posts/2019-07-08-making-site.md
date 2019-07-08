---
title: "사이트만들기"
date: 2019-07-08 11:57
---
처음에는
http://labs.brandi.co.kr/2018/05/14/chunbs.html
에서 공부하면서 ruby도 깔고 여러가지로 해봤다.
ruby를 설치하고, jekyll을 이용해서 로컬 호스트를 이용한 웹사이트를 만들었다.
gem install jekyll 로 설치하고
jekyll new my_blog 로 페이지를 만들고
cd my_blog 로 페이지 폴더에 들어가서
jekyll serve 로 페이지를 실행시킨다.
http://localhost:4000 이라는 주소로 웹을 볼 수 있다.

admin페이지를 통해 페이지를 쉽게 관리할 수 있었는데, my_blog폴더 안에 있는 Gemfile이라는 파일 안에
gem 'jekyll-admin', group: :jekyll_plugins 를 써주고
cmd에서 다시 bundle install을 해서 적용시켜주고 jekyll serve로 다시 웹을 실행시켜주면
http://localhost:4000/admin 에 들어갈 수 있는데, 포스팅 등의 것들을 쉽게 다룰 수 있다.

ruby를 사용하는 것도 처음이고 잘 모르겠어서 로컬 사이트 만들기까지는 했지만,
깃허브 페이지와 연결을 할 수가 없었다.

결국 https://dreamgonfly.github.io/2018/01/27/jekyll-remote-theme.html
이 글의 jekyll remote theme으로 github에서 곧바로 만드는 방법을 이용했다.
github에서 새로 username.github.io라는 respoistory를 만들고, remote theme 기능을 이용해서 
다른 jekyll 웹사이트 테마의 _config.yml 파일 내용을 복사해온 후 만들면 바로 웹이 만들어진다. 

이 방법으로 만든 웹페이지에서는 아직 admin페이지를 만드는 법은 모르겠다. 추후 찾아볼것.
