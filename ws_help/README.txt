本文件夹下各文件以及子文件夹的说明：
# index.html : 帮助页面的总目录，后面以 “ 一级目录 ” 称之
# style.css : 所有页面公用的样式表，修改请慎重
  ## 建议所有原本使用 vh 为单位的长度值的地方，改动时保留以 vh 为单位
  ## 1vh 是窗口高度的百分之一
# README.txt : 本文件，说明当前目录的使用方式
# . : 当前文件夹
  ## 下面所有提及 “ 根文件夹 ” 时，都是指包含 index.html，style.css 和 README.txt 等项目的本文件夹
  ## 下面所有提及 “ 二级文件夹 ” 时，都是指直属于根文件夹的子文件夹
# images : 专用的二级文件夹，下面存放了所有页面中引用的图片素材，需要使用新的图片素材时，建议同样放置在本文件夹中
  ## 替换 images 中的素材即可更改整个帮助页面的画面细节
  ## 替换素材时，请先检查被替换的素材的长宽，新的素材首先需要保持长宽比，如果能保持原始的长宽值最佳
# level_2_template : 二级文件夹的模板
# level_2_template/index.html : 二级目录的模板
# level_2_template/content.html : 三级页面的模板
# 其它所有子文件夹 : 皆是以 level_2_template 为模板的二级文件夹，对应着根文件夹下的 index.html 中的一个条目

下面说明 “ 如何新增一个二级目录 ”
# 在当前文件夹下复制 level_2_template 文件夹，文件夹名称命名为以下划线连接的二级目录名称，以 introduce_resources 为例
# 进入复制出的二级文件夹（本例中是名为 introduce_resources 的子文件夹），用文本编辑器打开其中的 index.html
# 将打开的 index.html 中的 href="#" 的 a 标签之间的文本内容修改成将在导航栏展示给用户看的文本，本例中为 Introduce Resources
# 然后文本编辑器打开当前二级文件夹下的 content.html，将 href="./index.html" 的 a 标签之间的文本内容替换，本例中替换为 Introduce Resources
# 最后文本编辑器打开根文件夹下的 index.html，按 class="inner" 的 div 中所言的指示新增一个条目，至此一个二级目录创建完毕

下面说明 “ 如何新增一个三级页面 ”
# 进入要新增三级页面的二级文件夹，此处沿用上一说明中的 introduce_resources 的例子，复制其中的 content.html，把复制的文件命名为下划线连接的三级页面的标题，保持 html 后缀不变，以 what_is_wood 为例
# 文本编辑器打开上一步复制出的 html 文件，本例中是 what_is_wood.html，将 href="#" 的 a 标签以及 class="level_3_title" 的 div 标签之间的文本内容修改为页面标题，本例中为 What is wood?
# 文本编辑器打开当前二级文件夹下的 index.html 文件，按其中的指示新增一个条目，至此一个三级页面创建完毕

最后，如果要修改三级页面，打开对应的 html 文件，然后按照其中的注释中所言修改即可。
