function FindFulfilmentObject(entity)
	local targets = entity:SearchBackpack("Drinks")
	local bestDrink = 0
	local chosenDrink = nil
	
	--target is a MoonItem
	for key, target in ipairs(targets) do
		if target.Value > bestDrink then
			bestDrink = target.Value
			chosenDrink = target
		end
	end
	
	--If we've found a drink, drink it
	if chosenDrink != nil then
		entity:InteractWithItem(chosenDrink)
		entity:RemoveItemFromBackpack(chosenDrink)
		return
	end
	
	--If we don't find a drink in our pack, then we need to find one on the ground
	targets = entity:SearchForObject("Drinks")
	bestDrink = 0
	chosenDrink = nil		
		
	--target is a MoonItem
	for key, target in ipairs(targets) do
		if target.Value > bestDrink then
			bestDrink = target.Value
			chosenDrink = target
		end
	end
	
	--If there's a drink nearby, go find it
	if chosenDrink != nil then
		if chosenDrink:GetPosition().Equals(entity:GetPosition()) == true then
			entity:InteractWithItem(chosenDrink)
			entity:RemoveItemFromWorld(chosenDrink)
			return
		else
			entity:Seek(chosenDrink, "Drink")
			return
		end
	end
	
	--If there isn't a drink nearby, wander
	entity:Wander()
end

function Tick(entity)
end