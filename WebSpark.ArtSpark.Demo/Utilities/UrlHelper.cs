using Microsoft.AspNetCore.Mvc;
using WebSpark.ArtSpark.Demo.Models;

namespace WebSpark.ArtSpark.Demo.Utilities;

/// <summary>
/// Helper class for generating SEO-friendly URLs for collections and collection items
/// </summary>
public static class UrlHelper
{
    /// <summary>
    /// Generates a SEO-friendly URL for a collection
    /// </summary>
    /// <param name="urlHelper">The URL helper instance</param>
    /// <param name="collection">The collection to generate URL for</param>
    /// <returns>SEO-friendly collection URL</returns>
    public static string CollectionUrl(IUrlHelper urlHelper, UserCollection collection)
    {
        return urlHelper.RouteUrl("collectionBySlug", new { slug = collection.Slug }) ?? $"/collection/{collection.Slug}";
    }

    /// <summary>
    /// Generates a SEO-friendly URL for a collection by slug
    /// </summary>
    /// <param name="urlHelper">The URL helper instance</param>
    /// <param name="slug">The collection slug</param>
    /// <returns>SEO-friendly collection URL</returns>
    public static string CollectionUrl(IUrlHelper urlHelper, string slug)
    {
        return urlHelper.RouteUrl("collectionBySlug", new { slug = slug }) ?? $"/collection/{slug}";
    }

    /// <summary>
    /// Generates a SEO-friendly URL for a collection item
    /// </summary>
    /// <param name="urlHelper">The URL helper instance</param>
    /// <param name="collection">The collection containing the item</param>
    /// <param name="item">The collection item</param>
    /// <returns>SEO-friendly collection item URL</returns>
    public static string CollectionItemUrl(IUrlHelper urlHelper, UserCollection collection, CollectionArtwork item)
    {
        if (string.IsNullOrEmpty(item.Slug))
        {
            // Fallback to ID-based URL if no slug is available
            return $"/collection/{collection.Slug}#artwork-{item.ArtworkId}";
        }

        return urlHelper.RouteUrl("collectionItemBySlug", new
        {
            collectionSlug = collection.Slug,
            itemSlug = item.Slug
        }) ?? $"/collection/{collection.Slug}/item/{item.Slug}";
    }

    /// <summary>
    /// Generates a SEO-friendly URL for a collection item by slugs
    /// </summary>
    /// <param name="urlHelper">The URL helper instance</param>
    /// <param name="collectionSlug">The collection slug</param>
    /// <param name="itemSlug">The item slug</param>
    /// <returns>SEO-friendly collection item URL</returns>
    public static string CollectionItemUrl(IUrlHelper urlHelper, string collectionSlug, string itemSlug)
    {
        return urlHelper.RouteUrl("collectionItemBySlug", new
        {
            collectionSlug = collectionSlug,
            itemSlug = itemSlug
        }) ?? $"/collection/{collectionSlug}/item/{itemSlug}";
    }

    /// <summary>
    /// Generates URL for collections listing
    /// </summary>
    /// <param name="urlHelper">The URL helper instance</param>
    /// <returns>Collections listing URL</returns>
    public static string CollectionsUrl(IUrlHelper urlHelper)
    {
        return urlHelper.RouteUrl("collections") ?? "/collections";
    }

    /// <summary>
    /// Generates URL for public collections explore page
    /// </summary>
    /// <param name="urlHelper">The URL helper instance</param>
    /// <returns>Public collections URL</returns>
    public static string PublicCollectionsUrl(IUrlHelper urlHelper)
    {
        return urlHelper.RouteUrl("publicCollections") ?? "/explore/collections";
    }

    /// <summary>
    /// Generates URL for featured collections page
    /// </summary>
    /// <param name="urlHelper">The URL helper instance</param>
    /// <returns>Featured collections URL</returns>
    public static string FeaturedCollectionsUrl(IUrlHelper urlHelper)
    {
        return urlHelper.RouteUrl("featuredCollections") ?? "/explore/featured";
    }

    /// <summary>
    /// Generates canonical URL for a collection
    /// </summary>
    /// <param name="request">The HTTP request</param>
    /// <param name="collection">The collection</param>
    /// <returns>Canonical URL</returns>
    public static string GetCanonicalUrl(HttpRequest request, UserCollection collection)
    {
        var scheme = request.Scheme;
        var host = request.Host.Value;
        return $"{scheme}://{host}/collection/{collection.Slug}";
    }

    /// <summary>
    /// Generates canonical URL for a collection item
    /// </summary>
    /// <param name="request">The HTTP request</param>
    /// <param name="collection">The collection</param>
    /// <param name="item">The collection item</param>
    /// <returns>Canonical URL</returns>
    public static string GetCanonicalUrl(HttpRequest request, UserCollection collection, CollectionArtwork item)
    {
        var scheme = request.Scheme;
        var host = request.Host.Value;

        if (string.IsNullOrEmpty(item.Slug))
        {
            return $"{scheme}://{host}/collection/{collection.Slug}#artwork-{item.ArtworkId}";
        }

        return $"{scheme}://{host}/collection/{collection.Slug}/item/{item.Slug}";
    }
}
