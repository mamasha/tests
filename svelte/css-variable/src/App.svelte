<script lang="ts">
	function rand(max: number) {
		return Math.floor(Math.random() * max);
	}


	let colors: string[] = [
		'red', 'blue', 'green'
	];

	let color: string = colors[0];

	function setRandColor() {
		while (true)
		{
			let next = colors[rand(colors.length)];
			if (next != color)
				return next;
		}
	}

	setInterval(setRandColor, 1000);
</script>

<div style="--color: {color}">
	<p>
		the color is set using a CSS variable
	</p>
</div>

<input type="color" bind:value={color} style="height: 50px;">

<style>
	p {
		color: var(--color);
	}
</style>