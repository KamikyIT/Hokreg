   M S B A M L   `   `   `  ����H  Dhokreg, Version=0.1.6155.34221, Culture=neutral, PublicKeyToken=nullE'clr-namespace:Uniso.InStat.StreamPlayerUniso.InStat.StreamPlayer  5      G(clr-namespace:Uniso.InStat.Gui.WPF_FormsUniso.InStat.Gui.WPF_Forms  5      Q-clr-namespace:Uniso.InStat.Gui.WPF_Forms.MVVMUniso.InStat.Gui.WPF_Forms.MVVM  5      2    ,Uniso.InStat.Gui.WPFForms.WPFMainFormControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      CwpfStreamPlayer'clr-namespace:Uniso.InStat.StreamPlayer     5      :local(clr-namespace:Uniso.InStat.Gui.WPF_Forms     5      C	viewModel-clr-namespace:Uniso.InStat.Gui.WPF_Forms.MVVM     5        � 	Resources  5         BlueUnderLineButton �� 
TargetType  Margin3Button ! MyRecoloredTestButtonTemplate �� CornerRadius � Offset � Color��%�  &	        &	 R     &	 �     �� 5   
   " ��66   .R�5      �� 
����  �$��	Underline{�6*   5      �� 5   
   " ��60   .R�5      �� $��3q�6   5      �� 5   
   " ��6@   .R�5      �� 
����+��$��3q�6.   $ 3��6B   
���������5      �� 5      .��5      � � 5      $ 0Q�6+   $ 	LimeGreen��66   6J   � 5      $ 1Q�6+   $ 	LightBlue��66   6J   5      5      5      5      5      .��5!      � -   5         � RowDefinitions 5"   
   �� 5#      $��1* �6   �� 5$      $��3* �6   �� 5%      $��1* �6    � ColumnDefinitions 5'   
   �� 5(      $��1* �6   �� 5)      $��3* �6   �� 5*      $��1* �6   .+�5.   
   ��  K� ZIndex$	 100��6   $��1��6   $��1��6.   $��Stretch=�6;   $��Stretch=�6W   .+�5/      �� -   5        
 �� Top$	 -20��5/      	 �� Left$		 105��6;   .��6M   PLAY6S   �� 50      -   5        $��DirtyTimeTextBlock��50      $	 -20��62   $	��100��6d   
����� �
���   ���6C   �� *DirtyTimeString+6�   4   .Uniso.InStat.StreamPlayer.StreamVideoPlayerWpf  51      -   5        
  Name$
 streamVideoPlayerWpf��51   3   $��Stretch=�6Q   $��Stretch=�6o   $	��520��6�   $	��940��6�   $	 10��6�   $ 10��6�   52      �� 54   
   $��0��6   $��0��6"   $��3��62   $��3��6F   .@�55      �� $��ТТД��6   $��0��6#   .��56      �� 
����d�
����d�._�57      ��  �� Orientation$ Vertical=�6!   .+�58      �� $ 
Horizontal=�6%   $��0��6>   $��Center=�6K   .+�59      �� -   5        $��
TestButton��59   s   $��15q�6%   ��61   �� *OpenVideoFileCommand+.��5:   "   �� Открыть видео6;   5;      �� 5<      
���   �$��2q�69   ._�5=   "   �  5>   &   �� 5?   *   �� 5@   *   .+�5B   &   �� $ 
Horizontal=�61   $��0��6J   $��Center=�6W   $��Center=�6t   $��5q�6�   .+�5C   *   �� $��3q�61   $��Period1��6f   ��6<   �� *RegisterMarkerCommand+.��5D   .   �� 
Period 16B   5E   +   �� 5F   *   $��3q�61   $��Period2��6f   ��6<   �� *RegisterMarkerCommand+.��5G   .   �� 
Period 26B   5H   +   �� 5I   *   $��3q�61   $��Period3��6f   ��6<   �� *RegisterMarkerCommand+.��5J   .   �� 
Period 36B   5K   +   �� 5L   *   $��3q�61   $��Penalty��6f   ��6<   �� *RegisterMarkerCommand+.��5M   .   �� 	Penalty6A   5N   +   �� 5O   *   $��3q�61   $��Overtime��6f   ��6<   �� *RegisterMarkerCommand+.��5P   .   �� 
Overtime6B   5Q   +   5R   '   �� 5S   &   $ 
Horizontal=�61   $��1��6J   $
��Left=�6W   $��Center=�6r   $��5q�6�   .+�5T   *   �� $��Center=�61   $��3q�6L   .��5U   .   �� $��Center=�68   $��3q�6S   Перерыв6g   5V   +   �� 5W   *   $��3q�61   .��5X   .   �� $��Center=�68   $��3q�6S   Конец матча6k   5Y   +   5Z   '   5[   #   5\      �� 5]      $��Center=�6'   $��3q�6B   .��5^   "   �� $��Center=�6,   $��3q�6G   ;9Останавливать видео на бросках6r   5_      �� 5`      $��15, 20q�6%   $��Right=�65   .��5a   "   �� $��3q�6,   Сохранить6B   5b      5c      �� 5d      $ 
Horizontal=�6%   $��1��6>   $��Center=�6K   .+�5f      �� $��Center=�6(   $��3q�6C   RATE6T   �� 5g      -   5        $��VideoRateSlider��5g   %   $��Center=�6<   $��3q�6W    �� IsSnapToTickEnabled .  � Minimum$ 0Q�6~    � Maximum$ 4Q�6�    �� Orientation$ 
Horizontal=�6�    �� TickFrequency$
 0.25Q�6�    � Value$ 1Q�6�    �� TickPlacement$ BottomRight=�6�   $	��100��6�   5h      �� 5i      $��Center=�6(   $��3q�6C   x1.06U   �� 5j      $��Center=�6(   $��3q�6C   Vol.6T   �� 5k      -   5        $��VolumeSlider��5k   %   $��Center=�69   $��3q�6T   $ 0Q�6_   $ 1Q�6k   $ 
Horizontal=�6w   $	 0.1Q�6�   $ 1Q�6�   $ BottomRight=�6�   $	��100��6�   5l      �� 5n      
������ $��3q�6>   .��5o   "   �� 
����  �$��	Underline{�6>   Настройки6e   5p      �� 5q      
������ $��3q�6>   .��5r   "   �� 
����  �$��	Underline{�6>   0.Поменять команды местами6t   5s      5u      5v      5w      5y      �� 5z      $��Замены��6   $��0��6&   5{      �� 5|      $��Полное ТТД��6   $��0��6*   5}      5~      3clr-namespace:Uniso.InStat.GuiUniso.InStat.Gui  5�      �� 5�   
   F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5�      :x,http://schemas.microsoft.com/winfx/2006/xaml     5�      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5�      @d2http://schemas.microsoft.com/expression/blend/2008     5�      0localclr-namespace:Uniso.InStat.Gui     5�      $��2��5�      $��1��6    $��12(�5�      ��5�   0   �� * MarkerWithPropertiesCollection+��5�      �� 5�       �� 
TargetType" ��6    �� Triggers 5�      �� 5�       �� Binding 6&   �� *
FlagUpdate+ �� Value true6E   .a�5�      ��  �� Property � �� �� Value
 �   �6;   �� 5�       � ��
 �����6;   5�      �� 5�       6&   �� *
FlagUpdate+	 false6E   .a�5�      ��  � ��
 �   �6;   �� 5�       � ��
 �����6;   5�      �� 5�       6&   �� *	FlagSaved+	 false6D   .a�5�      ��  � ��
 �   �6;   �� 5�       � ��
 �����6;   5�      �� 5�       6&   �� *IsReviewError+ true6H   .a�5�      ��  � ��
 �����6;   �� 5�       � ��
 �  ��6;   5�      �� 5�       6&   �� *
IsCritical+ true6E   .a�5�      ��  � ��
 �����6;   �� 5�       � ��
 � ���6;   5�      �� 5�       6&   �� *	IsWarning+ true6D   .a�5�      ��  � ��
 �   �6;   �� 5�       � ��
 � ���6;   5�      �� 5�       6&   �� *	FlagDel+ true6B   .a�5�      ��  � ��
 �����6;   �� 5�       � ��
 �����6;   5�      P� 5�       P� Property �@��
IsSelected P� Value . . �5�      ��  � ��
 �����6;   �� 5�       � ��
 ��  �6;   5�      5�       �� View 5�      �� 5�      .P�5�      �� $��ID��6I    �� Width$ 60��6U    �� DisplayMemberBinding 6%   �� *Id+6U   �� 5�      $��Action��6S   $	 180��6c    6%   �� *ActionString+6c   �� 5�      $��PERIOD��6M   $ 20��6]    6%   �� *Period+6]   �� 5�      $
��TIME��6Q   $ 60��6_    6%   �� *
TimeString+6_   �� 5�      $��PLAYER��6N   $	 150��6^    6%   �� *	Player1+6^   �� 5�      $��TEAM1��6L   $ 70��6[    6%   �� *Team1+6[   �� 5�      $��OPPONENT��6N   $	 150��6`    6%   �� *	Player2+6`   �� 5�      $��TEAM2��6L   $ 70��6[    6%   �� *Team2+6[   �� 5�      $
��POS1��6Q   $ 80��6_    6%   �� *
Pos1String+6_   �� 5�      $
��POS2��6Q   $ 80��6_    6%   �� *
Pos2String+6_   5�      5�      5�      5�      