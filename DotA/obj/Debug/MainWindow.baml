   M S B A M L   `   `   `  ����?  ;DotA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:DotADotA  5   	       DotA.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   $localclr-namespace:DotA     5   	   -   5          � Title$  
MainWindow��5   	    � SizeToContent$ WidthAndHeight=�5	       � WindowStyle$
 None=�5
       � WindowStartupLocation$ CenterScreen=�5       � WindowState$ Normal=�5       � 
ResizeMode$ NoResize=�5       � AllowsTransparency . 
������ .��5      � .+�5      �� -   5         ��Name$ BMain��5      $��0q�6   
���   � �� CornerRadius$ 50��6A   
���   �._�5      � -   5        	 �Name$	 GMain��5   
   $
��1000��6   $	��700��6)   
 � RowDefinitions
 5      �� 5      $
��50px �6   �� 5      $��1* �6   �� 5      $
��50px �6    � ColumnDefinitions 5      �� 5      $
��50px �6   �� 5      $��1* �6   �� 5      $��100px �6   .+�5      �� -   5        $
 BTop��5      $��1��6   $��0��6+   
���   � K� ZIndex$ 1��6K   �� 5      -   5        $ BBottom��5      $��1��6   $��2��6.   
���   �$ 1��6N   �� 5       -   5        $ BLeft��5       $��0��6   $��1��6,   
���   �$ 1��6L   �� 5!      -   5        $ BRight��5!      $��2��6   $��1��6-   
���   �$ 1��6M   ._�5"      � 
 5#      �� 5$   	   $��1* �6   �� 5%   	   $��100px �6   �� 5&   	   $��1* �6   .+�5(      �� -   5        $ BExit��5(      $��1��6   $��Stretch=�6+   $��Stretch=�6I   ��5)   	   �� 5*   
    �� ImageSource$ img/ButtonOffBG.png��6   5,   	   5-      5.       � 5/      -	   5          �Name$ FMain��5/      $��Stretch=�6   $��Stretch=�69   $��1��6U   $��1��6e     � NavigationUIVisibility$ Hidden=�6r   50      51      52      53      