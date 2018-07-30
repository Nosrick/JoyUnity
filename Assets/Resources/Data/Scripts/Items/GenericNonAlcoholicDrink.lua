function Interact(entity, item)
	entity:FulfillNeed("Drink", item.Value)
	entity:RemoveItemFromBackpack(item)
end