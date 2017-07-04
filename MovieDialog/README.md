# Neural Movie QAQ
NeuralMovieQAQ是一款多轮对话系统，通过多轮对话不断确认用户的意图从而向用户推荐电影。这一过程在任务导向的多轮对话系统中通常被称为“slot填充”

## 使用方法
 - 向Bot表达想看电影的欲望，触发对话trigger
如：
   > B:我想看电影\来部电影\推荐部电影
   > B:我想看一部经典的电影\给我推荐一部动作片
   > B:我想看刘德华和张艺谋的片子\来部法国电影吧
   > B:我想看天下无贼(直接成功)

  更多测试用例Resources/userquery.txt和Resources/usersession.txt

 - Bot在演员、导演、类型、国家、上映时间这五个slot中random walk，在决定了要询问的slot后统计目前符合要求的电影，从中统计该slot可能值，将这些可能值提问给用户作为提示
Bot和User对话如：
   > B:想看经典的电影还是最近的电影呢
   > U:最近有哪些电影啊
   > B:想看哪个演员的电影呢
   > U:我想看王宝强的电影
   > B:想看哪个国家的电影呢
   > U:日本的吧
   > B:刘德华演了很多剧情片、 动作片、 喜剧片，想看哪种呢
   > U:剧情片
   > U:爱情片也行呢
   > B:刘德华和王晶、 杜琪峯、 杜琪峰有很多合作，想看谁的呢
   > U:王晶的
   > U:有徐克的吗

 - 当Bot在五个slot中填满两个时开始推荐候选电影，一共推荐三轮，第一轮一部，第二轮两部，第三轮三部
Bot提出候选后，用户可以接受或拒绝，拒绝后进入下一轮推荐，接受后对话结束
   > B:你想看 澳门风云2 吗
   > U:行啊\好\可以\就他\
   > B:你想看 王牌逗王牌 或者 拆弹专家 吗
   > U:不想看\换一拨\那就第一个吧
   > B:你想看 未来警察 或者 超级学校霸王 或者 王牌逗王牌 吗
   > U:第一个吧\未来警察不错\未来警察

 - 用户在推荐过程中会有问题，比如用户想知道刘德华演过哪些电影，天下无贼是讲什么的。所以对话过程全程支持KBQA(推荐过程中，确认过程中，演员导演澄清过程中)，QA范围支持电影的演员、导演、上映日期、评分、国家、故事情节和演员演过哪些电影、导演拍过哪些电影，主要支持语言见Resources/QA_pattern.txt
有时候数据库中没有相关数据，于是返回“数据库中没有相关的答案”
   > U:天下无贼是谁演的
   > U:天下无贼是谁拍的
   > U:给我介绍一下天下无贼
   > U:天下无贼什么时候上映的
   > U:刘德华演过什么电影
   > U:张艺谋拍过什么电影
   > U:刘德华和王晶合作过什么电影(搞不定，两个限定词几乎没有AI系统可以实现)
KBQA不会影响对话流程

## 概念定义(理解之后方便更自如地使用和扩展程序)

### User Intent Classification
    1. Need Movie Recommendation 想要进行电影推荐：
    如：
    想看刘德华的电影
    来一部刘德华的电影
    有没有好看的刘德华的电影
    给我推荐刘德华的电影
    来部刘德华的电影
    想看张艺谋的电影
    推荐一部张艺谋的电影
    有没有好看的日本电影
    有没有好看的台湾电影
    有什么好看的爱情电影
    给我推荐一部动作片
    我想看电影
    给我推荐部电影
    来部电影
    来部法国电影吧
    
    2. Provide Information 提供信息
    我想看动作片
    
    3. Need Knowledge-based QA 向Bot询问电影，演员导演的知识
    刘德华演过什么电影
    天下无贼是讲什么的
    可见Knowledge-Based Qusetion Answering
    
    4. Accept Candidate 接受Bot提议的电影
    这部电影可以
    那就这部吧
    听上去不错
    
    5. Deny Candidate 否定Bot提议的电影
    不想看
    换一部

### Dialog Policy
    根据用户的五种Intent，机器会产生四种响应：
    1. Movie Recommendation 进行电影推荐
    响应用户“Need Movie Recommendation”Intent
    分析并记录用户“提供信息”Intent
    根据Dialog State Tracking进行决策Dialog State Tracking
    
    2. Clarify Ambiguous Words 澄清歧义
    响应“Provide Information”有歧义时进行澄清
    User：我想看周星驰的电影
    Bot：周星驰既是演员又是导演，你想看他演的还是导的？
    
    3. Knowledge-Based QA 回答用户的问题
    响应用户“Need Knowledge-based QA”Intent
    用户对推荐的东西感兴趣，于是提问：
    刘德华演过什么电影
    天下无贼是讲什么的
    详见Knowledge-Based Qusetion Answering
    
    4. Comfirm the Candidate 获取足够的信息后向用户展示结果并确认
    人类交谈时不会一次性问五个Slot的信息，因此我的系统也不会，那样太生硬
    因此我的系统在询问到2~3个Slot后返回结果让用户选择（或在候选结果<=5时终止）
    
    系统展示结果时需要用户确认，Bot响应用户“Accept Candidate”Intent和“Deny Candidate ”。一共展示三轮，第一轮一部，第二轮两部，第三轮三部
    Bot提出候选后，用户可以接受或拒绝，拒绝后进入下一轮推荐，接受后对话结束
