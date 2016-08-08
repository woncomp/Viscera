# Viscera
Viscera is basically a super inspector that can show a lot of stuffs that unity inspector won't. It will give you great convenience when you want a quick look of the states of your object. You will no longer need to set a breakpoint to watch the value or insert a Debug.Log to print it out.

**Features:**
+ Inspect mono object.
+ Inspect elements of List and Dictionary.
+ Change values on the fly.
+ Look and modify property values.
+ Realtime update during game running.
+ Modify struct members.
+ Group class members by inheritance level.

## Get Started
1.	Open Viscera Window through menu item Window/Viscera

![](https://github.com/woncomp/Viscera/blob/woncomp-patch-1/2016-08-03_23-10-32.png)

2.	Select the object you want to inspect. The object you are currently selecting is shown in the center of Viscera Window.
3.	Click ‘Go Inside’

## Introduction of The Interface
Most of the time in Viscera you are dealing with an interface like the picture below. Viscera lists the class members of the object that are currently focused. Each line displays a member’s name, value and type, letting you watch and modify the value in a fairly suitable manner.

![](https://github.com/woncomp/Viscera/blob/woncomp-patch-1/2016-08-03_23-17-14.png)

1.	**Tab Bar** – Viscera allows you to inspect multiple objects at the same time, you can click [+] to create a new tab and switch among them at any time.
2.	**Address Bar** – This indicates how did you get to the current object from the object you initially inspected. You can click on any level of the path and jump there immediately. Just like the file browser of your computer.
3.	**Go Inside Button** – If the type of the member is a class or struct, which could have a rather complicated structure, you can click this button and navigate to the inside of the member to see more details.
4.	**Inheritance Level Separator** – Viscera will group the members by inheritance level.
5.	**Show Value Button** – Evaluation of a C# property could introduce side effects. So here is a button, that Viscera will only evaluate the property after you pressed that.
