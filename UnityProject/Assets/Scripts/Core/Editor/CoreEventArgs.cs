namespace Core
{
	public class LayerEventArgs : EventArgs
	{
		public Layer layer;

		public LayerEventArgs(Layer layerParam)
		{
			layer = layerParam;
		}
	}
}