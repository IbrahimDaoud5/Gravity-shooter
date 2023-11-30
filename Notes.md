<h1>Versions </h1>
Unity Hub version: 3.6.1 <br>
Editor version: 2022.3.9f1 LTS <br>

<h2>If we will need more than 100MB for a file - Git LFS </h2>
This shows how to use git LFS ---> 
First of all, we need to navigate to the file/s path in git bash terminal, <br>
Then we write:<br> `$git lfs track "*.fileextension"` <br> (if we want all files from this type, or singlefiles)
<br><br> then if we write git status it will show us the tracked files, we can see .gitattributes that tells git which big files to track
<br>__Important:__ here is a video that provide us with a file that has all the big files that we will need listed in one place - https://www.youtube.com/watch?v=_ewoEQFEURg
