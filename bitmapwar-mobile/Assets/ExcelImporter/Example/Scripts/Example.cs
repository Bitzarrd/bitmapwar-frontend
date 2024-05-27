using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Example : MonoBehaviour
{
	[SerializeField] MstItems mstItems;
	[SerializeField] Text text;

	[SerializeField] TestItems testItems;
	
	void Start()
	{
		ShowItems();
	}

	void ShowItems()
	{
		string str = "";

		mstItems.Entities
			.ForEach(entity => str += DescribeMstItemEntity(entity) + "\n");

		text.text = str;
		str = "";
		testItems.Entities.ForEach(entity => str += Describe(entity) + "\n" );
		text.text += str;
		testItems.Entities.ForEach(entity => entity.price += 2);
	}

	string Describe(TestItemEntity entity)
	{
		return string.Format("{0}:,{1},{2}",entity.id,entity.name,entity.price);
	}

	string DescribeMstItemEntity(MstItemEntity entity)
	{
		return string.Format(
			"{0} : {1}, {2}, {3}, {4}, {5}",
			entity.id,
			entity.name,
			entity.price,
			entity.isNotForSale,
			entity.rate,
			entity.category
		);
	}
}

