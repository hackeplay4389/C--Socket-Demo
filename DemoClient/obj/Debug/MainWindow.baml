   M S B A M L   `   `   `  ����E  ADemoClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:DemoClient
DemoClient  5   	       DemoClient.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:DemoClient     5   	   -   5        $��20(�5        � Icon$  logo.ico��6(    � 
ResizeMode$ CanMinimize=�68    � Title$ Socket通信 - 客户端��5   	   $	��521��6    $	��580��6-    � WindowStartupLocation$ CenterScreen=�69    � 	Resources 5       
  dt_msg �� CornerRadius �� Orientation��%�  &	        �� 5   
   .R�5      �� $
��auto��6   $	��505��6#   
�������$	��0,5q�6G   $��1q�6T   $ 5��6h   ._�5      �� $ Vertical=�6   $
��Auto��64   $	��Top=�6B   .+�5      �� 
��� ���$��12(�6K   $	��7,0q�6Y   ��6   �� *Adress+6Y   �� 5      $��0q�65   $��12(�6@   
����  ���6   �� *Time+6N   �� 5      $
��auto��6    $
��Wrap=�6.   $��	10,0,10,5q�6W   $��15(�6j   ��6B   �� *Msg+6j   5      5      5      5      .��5      �� .+�5   
   �� $��主机IP：��6    �� Left$ 13��6    
 �� Top$ 16��61   �� 5   
   -   5        	 ��Name$	 txt_IP��5      $��Center=�6"   $��31��6D   $ 93��6P   
 �� TextWrapping$

 Wrap=�6a   $�� ��6u   $ 17��6}   $	��160��6�   �� 5   
   $��	端口：��6   $	 273��6   $ 17��60   �� 5   
   -   5        $	 txt_Port��5      $��Center=�6$   $��Center=�6F   $��31��6j   $	 333��6v   $

 Wrap=�6�   $
��8080��6�   $ 17��6�   $��80��6�   �� 5   
   -   5         ��Name$ btn_StartConn��5      $��16(�6(   $��连接主机��6S    � Cursor$
 Hand��6b   $	 438��6p   $ 16��6�   $	��115��6�   $��31��6�   �� 5   
   $��消息记录：��6   $ 13��6    $ 56��61   �� 5    
   -   5         ��Name$ lb_msg��5       $	��249��6"   #��[  60   $ 13��6X   $ 96��6i   $	��540��6y   �� 5!   
   -   5        $	 txt_Send��5!      $��15(�6$   $��70��62   $ 13��6>   $

 Wrap=�6O   $�� ��6c   $	 360��6k   $	��540��6|   �� 5"   
   -   5        $ btn_SendMsg��5"      $��16(�6A   $��发送消息��6P   $
 Hand��6_   $	 438��6m   $	 444��6   $	��115��6�   $��31��6�   5#      5$      