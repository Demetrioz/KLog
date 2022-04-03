// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require("prism-react-renderer/themes/github");
const darkCodeTheme = require("prism-react-renderer/themes/dracula");

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: "KLog",
  tagline: "Simple, developer-focused logging",
  url: "https://klogdocs.kevinwilliams.dev",
  baseUrl: "/",
  onBrokenLinks: "throw",
  onBrokenMarkdownLinks: "warn",
  favicon: "img/favicon.ico",
  organizationName: "Demetrioz",
  projectName: "KLog",
  deploymentBranch: "gh-pages",
  trailingSlash: false,

  presets: [
    [
      "classic",
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: require.resolve("./sidebars.js"),
          editUrl: "https://github.com/Demetrioz/KLog",
        },
        blog: {
          showReadingTime: true,
          editUrl: "https://github.com/Demetrioz/KLog",
        },
        theme: {
          customCss: require.resolve("./src/css/custom.css"),
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      navbar: {
        title: "KLog",
        logo: {
          alt: "KLog",
          src: "img/logo.png",
        },
        items: [
          {
            type: "doc",
            docId: "introduction",
            position: "left",
            label: "Documentation",
          },
          // { to: "/blog", label: "Blog", position: "left" },
          {
            href: "https://github.com/Demetrioz/KLog",
            label: "GitHub",
            position: "right",
          },
        ],
      },
      footer: {
        style: "dark",
        links: [
          {
            title: "Docs",
            items: [
              {
                label: "Introduction",
                to: "/docs/introduction",
              },
              {
                label: "Build",
                to: "docs/getting-started/build",
              },
            ],
          },
          {
            title: "Community",
            items: [
              {
                label: "Talkyard",
                href: "https://talkyard.kevinwilliams.dev/",
              },
              {
                label: "Discord",
                href: "https://discord.gg/qEZDFqF6xg",
              },
            ],
          },
          {
            title: "More",
            items: [
              {
                label: "kevinwilliams.dev",
                href: "https://kevinwilliams.dev",
              },
              {
                label: "GitHub",
                href: "https://github.com/Demetrioz/KLog",
              },
            ],
          },
        ],
        copyright: `Copyright © ${new Date().getFullYear()} Kevin Williams. Built with Docusaurus.`,
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
      },
    }),
};

module.exports = config;
