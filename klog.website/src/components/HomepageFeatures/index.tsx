import React from "react";
import clsx from "clsx";
import styles from "./styles.module.css";

type FeatureItem = {
  title: string;
  Svg: React.ComponentType<React.ComponentProps<"svg">>;
  description: JSX.Element;
};

const FeatureList: FeatureItem[] = [
  {
    title: "Easy to Use",
    Svg: require("@site/static/img/undraw_docusaurus_mountain.svg").default,
    description: (
      <>
        Built with React and .Net, KLog is designed to be easy to deploy as code
        or with containers.
      </>
    ),
  },
  {
    title: "Developer Focused",
    Svg: require("@site/static/img/undraw_docusaurus_tree.svg").default,
    description: (
      <>
        KLog is designeed with developers in mind. Simply create an API key and
        start sending logs. Receive GitHub events with a built-in endpoint and
        easy customization.
      </>
    ),
  },
  {
    title: "Expanding Feature Set",
    Svg: require("@site/static/img/undraw_docusaurus_react.svg").default,
    description: (
      <>
        As development continues, you can expect things such as real time
        alerts, monitoring capabilities, and more.
      </>
    ),
  },
];

function Feature({ title, Svg, description }: FeatureItem) {
  return (
    <div className={clsx("col col--4")}>
      <div className="text--center">
        <Svg className={styles.featureSvg} role="img" />
      </div>
      <div className="text--center padding-horiz--md">
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures(): JSX.Element {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
