function FindFulfillmentObject(entity)
	local targets = entity:SearchBackpack("Food")
	local bestFood = 0
	local chosenFood = nil
	
	--target is a MoonItem
	for key, target in ipairs(targets) do
		if target.Value > bestFood then
			bestFood = target.Value
			chosenFood = target
		end
	end
	
	--If we've found food, eat it
	if chosenFood != nil then
		chosenFood:Interact(entity)
		entity:RemoveItemFromBackpack(chosenFood)
		return
	end
	
	--If we don't find food in our pack, then we need to find one on the ground
	targets = entity:SearchForObject("Food")
	bestFood = 0
	chosenFood = nil		
		
	--target is a MoonItem
	for key, target in ipairs(targets) do
		if target.Value > bestFood then
			bestFood = target.Value
			chosenFood = target
		end
	end
	
	--If there's food nearby, go find it
	if chosenFood != nil then
		entity:Seek(chosenFood)
		return
	end
	
	--If there isn't food nearby, wander
	entity:Wander()
end