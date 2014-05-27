using UnityEngine;
using System.Collections;

public class RankingObjectScript : MonoBehaviour {

	public UILabel nameLabel;
	public UILabel battleStatusLabel;
	public UILabel levelLabel;
	public UILabel rankLabel;
	public UILabel victoryLabel;
	public UISprite backgroundSprite;
	public UISprite rankingIcon;
	private int rank = -1;

	public int Rank
	{
		set{
			rank = value;
			if(rank == 1 || rank == 2 || rank == 3)
			{
				rankLabel.gameObject.SetActive(false);
				rankingIcon.spriteName = "ranking_" + rank;
			}
			else
			{
				rankingIcon.gameObject.SetActive(false);
			}
		}
		get{ return rank; }
	}
}
