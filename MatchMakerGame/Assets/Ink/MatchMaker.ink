-> expressionLoop
=== receptionist ===
You enter an unfamiliar place. #narrator
//  #entrance:deer
#entrance:player
Hi. #player
#entrance:owl
Well, hello there!
Welcome to Match Maker. #exp:smile
Thanks. #player
Nice to meet you. #exp:frown
What's your name? #exp:notes
* [button text] my name is button. #player
    sjedfkjsenfg#player
    dslfkmv#player
* [I don't wan't to talk]
    I don't wan't to talk #player
* [I'm ok actually]
    I'm ok actually #player
-That's it. 
Still wondering what's going on, huh? #narrator
Oh, really? #player
Wow.
* [Challenge]
    // -> knot
* [Accept]
-Whatever.
-> DONE

=== expressionLoop
Here I am. #player
#entrance:player
Said the player. And made a choice... #narrator
Ok. #player
+ [Doe]
    #entrance:deer
    Hello!
+ [Owl]
    #entrance:owl
    Hello!
-Hello!
-> expr
-> DONE

=== expr 
+ [Happy]
    Yay! #exp:smile
+ [Sad]
    Aww. #exp:frown
+ [Notes]
    Interesting... #exp:notes
+ [change character]
    -> expressionLoop
--> expr
-> DONE

