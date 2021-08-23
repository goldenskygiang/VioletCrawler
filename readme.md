# Violet library crawler
## A small utility to automatically download lecture slides on Violet.vn

This tool crawls a list of lecture slides on a subject-class basis, following the pattern that each subject-class contains a list of lessons for the whole academic year.

### Usage instruction

This tool only supports those links that satisfy the following conditions:
1. The link must be unique to a subject-class level (Math 6, English 7, etc.)
2. Each subject-class contains a list of lessons available as first-level direct links (referred as lesson links).
3. Inside each lesson link, there should be only a list of lecture links, and no other deeper-level links exist in the main content.

The following screenshot is an example (conditions are highlighted in the image). You can click the image to navigate to the link:
<a href="https://baigiang.violet.vn/category/ngu-van-6-1335.html">![](https://i.imgur.com/kuerpst.png)</a>

Also in the above link, the lesson link are shown below:
<a href="https://baigiang.violet.vn/lesson/con-rong-chau-tien-1335-1.html">![](https://i.imgur.com/ZwPDyx4.png)</a>

To use this tool, you must have an existing Violet account to download lectures, as the system requires users to have member points to do so.

1. Login to [baigiang.violet.vn](https://baigiang.violet.vn) and obtain the JSON cookie file of your session. You can use some Chrome extensions such as Cookie-Editor to get this JSON.
2. Paste the JSON cookie into the cookie.json file.
3. Paste the described link and type your favored author name.

The tool will download a whole list of lectures automatically. If your favorite author's lecture is not available, the tool will pick the one with the most download count **on the first page** of the lesson link for you.

### Dependencies
- HtmlAgilityPack (for parsing HTML elements)
- Newtonsoft.Json (for parsing cookie text)

### Limitations
Since I only spent 2 hours to write this rarely-use tool, and approximately 1 hour to write this readme, there is always room for improvement. I will write down some for example:
- This tool only handles those links satifying the above descriptions.
- This tool does not use a headless browser, but use a static HTML parser instead. Some startup JavaScript code in HTML pages will not run, and therefore some content is missing. Initially, I intended to download lectures with the most number of slides but I was lazy to do it.
- This tool does not support pagination. Each lesson link may have multiple numbered pages, but again I was lazy to handle them.
- This tool does not automatically login using input username and password, instead use the cookie file.

However, it will still be useful if you have a relative working as a teacher. Feel free to play around with it and make some modification, as well as making use of HTML crawler.