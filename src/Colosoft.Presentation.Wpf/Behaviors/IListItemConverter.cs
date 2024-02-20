namespace Colosoft.Presentation.Behaviors
{
    public interface IListItemConverter
    {
        object Convert(object masterListItem);

        object ConvertBack(object targetListItem);
    }
}
