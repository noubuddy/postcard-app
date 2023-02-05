<script>
    import { createEventDispatcher } from 'svelte';

    const dispatch = createEventDispatcher();

    let file;
    let imageSrc;

    async function handleSubmit(event) {
        event.preventDefault();

        if (!file) return;

        const formData = new FormData();
        formData.append('Image', file, file.name);

        const response = await fetch('http://localhost:3000/api/Data/images', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            const error = await response.text();
            dispatch('error', { error });
            return;
        }

        const path = await response.text();
        dispatch('success', { path });
        imageSrc = path;
    }

    function handleFileChange(event) {
        file = event.target.files[0];
    }
</script>

<form on:submit={handleSubmit}>
    <input type="file" on:change={handleFileChange} />
    <button type="submit">Upload</button>
</form>

{#if imageSrc}
    <img src={imageSrc} />
{:else}
    <p>No file uploaded yet</p>
{/if}