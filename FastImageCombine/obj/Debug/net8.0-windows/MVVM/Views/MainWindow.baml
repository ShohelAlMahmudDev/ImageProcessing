   M S B A M L   `   `   `  ����K  GFastImageCombine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:FastImageCombineFastImageCombine  5   	   S.clr-namespace:FastImageCombine.MVVM.ViewModels FastImageCombine.MVVM.ViewModels  5   	   !    FastImageCombine.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:FastImageCombine     5   	   D	viewModel.clr-namespace:FastImageCombine.MVVM.ViewModels     5   	   $	��600��5	   
   $	��800��6     � DataContext  5
      4  .FastImageCombine.MVVM.ViewModels.MainViewModel  5   
   .��5      �� 
�������._�5   
   �  � ColumnDefinitions 5      �� 5      $	��150 �6#   61   �� 5      6%    � RowDefinitions 5      �� 5      $��75 �6    6.   �� 5      6"   .+�5      �� 
�������._�5      �� $
��Logo��6   $��40(�6(   
�������
�������$��Center=�6[   $��Center=�6v   5      �� 5      
�������$��1��6(   ._�5      �� $��Fast Image Combine��6   $��40(�66   
�������
�������$��Center=�6i   $��Center=�6�   5      �� 5      $��1��6   .+�5       	� $
��Home��6   $��50��6-   
�������   MenuButtonTheme#��[  6L    d� 	IsChecked$
 TrueY�6u   ��6�   �� *HomeViewCommand+	� 5!      $��Settings��6   $��50��61   
�������#��[  6P   ��6y   �� *SettingsViewCommand+	� 5"      $��Image Combine��6   $��50��66   
�������#��[  6U   ��6~   �� *ImageCombineViewCommand+5#      �� 5$      $��1��6   $��1��6-   ��6:   �� *CurrentView+5%      5&      5(      