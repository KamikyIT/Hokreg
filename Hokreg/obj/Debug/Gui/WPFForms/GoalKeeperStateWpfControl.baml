   M S B A M L   `   `   `  ����H  Dhokreg, Version=0.1.6155.34221, Culture=neutral, PublicKeyToken=nullE'clr-namespace:Uniso.InStat.Gui.WPFFormsUniso.InStat.Gui.WPFForms  5      9    3Uniso.InStat.Gui.WPFForms.GoalKeeperStateWpfControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:Uniso.InStat.Gui.WPFForms     5      .��5	      � -   5        $��MainGrid��5	        � RowDefinitions  5   
   �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      �� 5      �� 5      .+�5    
   (� -   5        $��nad_blin��5    C   $��0��6   $��0��6   $��2��6/   .��5!      � .+�5"      A� -   5        $��nad_blin_ellipse��5"      
�������
����� �$��2��6N   $��Stretch=�6b   $��Stretch=�6�   �� 5#      $��15q�6    �� TextAlignment$ Center=�6(   Над блином6K   5$      5%      (� 5'   
   -   5        $��nad_trap��5'   C   $��0��6   $��3��6   $��2��6/   .��5(      � .+�5)      A� -   5        $��nad_trap_ellipse��5)      
�������
����� �$��2��6N   $��Stretch=�6b   $��Stretch=�6�   �� 5*      $��15q�6   $ Center=�6(   Над ловушкой6M   5+      5,      (� 5.   
   -   5        $��v_telo��5.   C   $��3��6   $��1��6   $��3��6/   .��5/      � .+�50      A� -   5        $��v_telo_ellipse��50      
�������
����� �$��2��6L   $��Stretch=�6`   $��Stretch=�6~   �� 51      $��15q�6   $ Center=�6(   В тело6G   52      53      (� 55   
   -   5        $��pod_blin��55   C   $��4��6   $��0��6   $��2��6/   .��56      � .+�57      A� -	   5        $��pod_blin_ellipse��57      
�������
����� �$��2��6N   $��Stretch=�6b   $��Stretch=�6�   �� 58      $��15q�6   $ Center=�6(   Под блином6K   59      5:      (� 5<   
   -
   5        $��pod_trap��5<   C   $��4��6   $��3��6   $��2��6/   .��5=      � .+�5>      A� -   5        $��pod_trap_ellipse��5>      
�������
����� �$��2��6N   $��Stretch=�6b   $��Stretch=�6�   �� 5?      $��15q�6   $ Center=�6(   Под ловушкой6M   5@      5A      (� 5C   
   -   5        $��v_dom��5C   /   $��6��6   $��2��6   .��5D      � .+�5E      A� -   5        $��v_dom_ellipse��5E      
�������
����� �$��2��6L   $��Stretch=�6`   $��Stretch=�6~   �� 5F      $��15q�6   $ Center=�6(   	В дом6F   5G      5H      5J      5K      