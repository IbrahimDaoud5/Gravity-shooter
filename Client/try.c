#include<stdio.h>

int main()
{
int v1,v2,v3,v4,v5,v6,v7,v8;
int lastdig=3,i,sum;
printf("Please Enter your first 8 dig of ID:\n");
scanf("%d",&v1);scanf("%d",&v2);scanf("%d",&v3);scanf("%d",&v4);
scanf("%d",&v5);scanf("%d",&v6);scanf("%d",&v7);scanf("%d",&v8);
v2=2*v2;
if(v2>9)
v2=(v2/10)+(v2%10);
v4=2*v4;
if(v4>9)
v4=(v4/10)+(v4%10);
v6=2*v6;
if(v6>9)
v6=(v6/10)+(v6%10);
v8=2*v8;
if(v8>9)
v8=(v8/10)+(v8%10);
sum=v1+v2+v3+v4+v5+v6+v7+v8;
lastdig=sum%10;

printf("The last Digit is: %d",(10 - lastdig));

return 0;
}